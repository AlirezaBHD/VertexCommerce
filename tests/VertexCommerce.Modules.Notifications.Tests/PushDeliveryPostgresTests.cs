using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Persistence.Repositories;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Delivery;
using VertexCommerce.Shared.IntegrationEvents;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class PushDeliveryPostgresTests
{
    [NotificationsPostgresFact]
    public async Task A_retry_respects_the_provider_schedule_while_another_subscription_completes()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        var ids = await SeedDeliveriesAsync(host, 2, ct);
        var now = DateTimeOffset.UtcNow.AddMinutes(1);
        var clock = new TestClock { Now = now };
        var retryAt = now.AddMinutes(3);
        var sender = new FakeSender(subscription => subscription.Id == ids[0] && clock.Now < retryAt
            ? new WebPushSendResult(WebPushSendStatus.Retryable, 429, retryAt)
            : new WebPushSendResult(WebPushSendStatus.Accepted, 201));
        var processor = new Func<Task<bool>>(() => ProcessNextAsync(host.Services, sender, clock, ct));

        Assert.True(await processor());
        Assert.True(await processor());
        Assert.False(await processor());
        var jobs = await ReadJobsAsync(host.Services, ct);
        var retrying = Assert.Single(jobs, job => job.SubscriptionId == ids[0]);
        var accepted = Assert.Single(jobs, job => job.SubscriptionId == ids[1]);
        Assert.Null(retrying.Outcome);
        Assert.Equal(429, retrying.LastHttpStatus);
        Assert.Equal(1, retrying.Attempts);
        Assert.Equal(retryAt.ToUnixTimeMilliseconds(), retrying.NextAttemptAt.ToUnixTimeMilliseconds());
        Assert.Equal(PushDeliveryOutcome.Accepted, accepted.Outcome);

        clock.Now = retryAt.AddSeconds(-1);
        Assert.False(await processor());
        clock.Now = retryAt.AddSeconds(1);
        Assert.True(await processor());
        Assert.False(await processor());
        Assert.Equal(3, sender.Calls);
        jobs = await ReadJobsAsync(host.Services, ct);
        Assert.All(jobs, job => Assert.Equal(PushDeliveryOutcome.Accepted, job.Outcome));
        Assert.Equal(2, Assert.Single(jobs, job => job.Id == retrying.Id).Attempts);
    }

    [NotificationsPostgresFact]
    public async Task Concurrent_claims_get_one_lease_and_a_stopped_worker_can_be_replaced()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        Assert.Single(await SeedDeliveriesAsync(host, 1, ct));
        var now = DateTimeOffset.UtcNow.AddMinutes(1);

        var claims = await Task.WhenAll(Enumerable.Range(0, 10)
            .Select(_ => ClaimAsync(host.Services, now, ct)));
        var first = Assert.Single(claims.OfType<PushDelivery>());
        Assert.Null(await ClaimAsync(host.Services, now.AddSeconds(59), ct));
        var reclaimed = Assert.IsType<PushDelivery>(await ClaimAsync(host.Services, now.AddMinutes(1), ct));
        Assert.Equal(first.Id, reclaimed.Id);
        Assert.NotEqual(first.LeaseId, reclaimed.LeaseId);

        await using var scope = host.Services.CreateAsyncScope();
        var queue = scope.ServiceProvider.GetRequiredService<PushDeliveryQueue>();
        Assert.False(await queue.SetResultAsync(first, 1, 201, now, PushDeliveryOutcome.Accepted, now, null, ct));
        Assert.True(await queue.SetResultAsync(reclaimed, 1, 201, now, PushDeliveryOutcome.Accepted, now, null, ct));
        Assert.Equal(PushDeliveryOutcome.Accepted, Assert.Single(await ReadJobsAsync(host.Services, ct)).Outcome);
    }

    [NotificationsPostgresFact]
    public async Task Removed_and_expired_subscriptions_finish_without_blocking_the_remaining_delivery()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        var ids = await SeedDeliveriesAsync(host, 3, ct);
        await using var scope = host.Services.CreateAsyncScope();
        var subscriptions = scope.ServiceProvider.GetRequiredService<IPushSubscriptionRepository>();
        var removed = Assert.IsType<PushSubscription>(await subscriptions.GetByIdAsync(ids[0], ct));
        await subscriptions.RemoveIfUnchangedAsync(removed, ct);
        var sender = new FakeSender(subscription => subscription.Id == ids[1]
            ? new WebPushSendResult(WebPushSendStatus.SubscriptionExpired, 410)
            : new WebPushSendResult(WebPushSendStatus.Accepted, 201));
        var clock = new TestClock { Now = DateTimeOffset.UtcNow.AddMinutes(1) };

        Assert.True(await ProcessNextAsync(host.Services, sender, clock, ct));
        Assert.True(await ProcessNextAsync(host.Services, sender, clock, ct));
        Assert.True(await ProcessNextAsync(host.Services, sender, clock, ct));
        Assert.False(await ProcessNextAsync(host.Services, sender, clock, ct));

        var jobs = await ReadJobsAsync(host.Services, ct);
        Assert.Equal(PushDeliveryOutcome.SubscriptionRemoved, Assert.Single(jobs, job => job.SubscriptionId == ids[0]).Outcome);
        Assert.Equal(PushDeliveryOutcome.SubscriptionExpired, Assert.Single(jobs, job => job.SubscriptionId == ids[1]).Outcome);
        Assert.Equal(PushDeliveryOutcome.Accepted, Assert.Single(jobs, job => job.SubscriptionId == ids[2]).Outcome);
        Assert.Equal(2, sender.Calls);
        Assert.Null(await subscriptions.GetByIdAsync(ids[1], ct));
        Assert.NotNull(await subscriptions.GetByIdAsync(ids[2], ct));
    }

    [NotificationsPostgresFact]
    public async Task Attempt_limit_stops_rescheduling_a_failing_delivery()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        Assert.Single(await SeedDeliveriesAsync(host, 1, ct));
        var clock = new TestClock { Now = DateTimeOffset.UtcNow.AddMinutes(1) };
        var sender = new FakeSender(_ => new WebPushSendResult(WebPushSendStatus.Retryable, 503));

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            Assert.True(await ProcessNextAsync(host.Services, sender, clock, ct));
            Assert.False(await ProcessNextAsync(host.Services, sender, clock, ct));
            var job = Assert.Single(await ReadJobsAsync(host.Services, ct));
            Assert.Equal(attempt, job.Attempts);
            clock.Now = job.NextAttemptAt.AddSeconds(1);
        }

        var completed = Assert.Single(await ReadJobsAsync(host.Services, ct));
        Assert.Equal(PushDeliveryOutcome.AttemptsExhausted, completed.Outcome);
        Assert.Equal(5, sender.Calls);
        Assert.False(await ProcessNextAsync(host.Services, sender, clock, ct));
    }

    [NotificationsPostgresFact]
    public async Task The_hosted_worker_dispatches_after_the_order_handler_returns()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        var sender = new BlockingSender();
        await using var host = await NotificationsTestHost.StartAsync(
            ct, postgresConnection: database.ConnectionString, hostedPushSender: sender);

        Assert.Single(await SeedDeliveriesAsync(host, 1, ct));
        var deliveryId = await sender.Entered.Task.WaitAsync(ct);
        var queued = Assert.Single(await ReadJobsAsync(host.Services, ct));
        Assert.Equal(deliveryId, queued.Id);
        Assert.Null(queued.CompletedAt);
        sender.Release.SetResult();

        PushDelivery completed;
        do
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50), ct);
            completed = Assert.Single(await ReadJobsAsync(host.Services, ct));
        }
        while (completed.CompletedAt is null);

        Assert.Equal(PushDeliveryOutcome.Accepted, completed.Outcome);
        Assert.Equal(1, completed.Attempts);
    }

    private static async Task<Guid[]> SeedDeliveriesAsync(NotificationsTestHost host, int subscriptionCount, CancellationToken ct)
    {
        await using var scope = host.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPushSubscriptionRepository>();
        var customerId = Guid.NewGuid();
        var ids = new Guid[subscriptionCount];
        for (var index = 0; index < subscriptionCount; index++)
        {
            var request = TestPushSubscription.Create($"{TestPushSubscription.Endpoint}-{index}");
            var subscription = PushSubscription.Create(customerId, request.Endpoint, request.P256dh, request.Auth);
            Assert.True(await repository.TryUpsertOwnedAsync(subscription, ct));
            ids[index] = subscription.Id;
        }

        await host.PublishAsync(new OrderCreatedIntegrationEvent(Guid.NewGuid(), customerId, "VC-100", 1000, "IRR"), ct);
        return ids;
    }

    private static async Task<PushDelivery[]> ReadJobsAsync(IServiceProvider services, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        return await context.Set<PushDelivery>().AsNoTracking().ToArrayAsync(ct);
    }

    private static async Task<PushDelivery?> ClaimAsync(IServiceProvider services, DateTimeOffset now, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<PushDeliveryQueue>()
            .ClaimAsync(now, PushDeliveryPolicy.LeaseDuration, ct);
    }

    private static async Task<bool> ProcessNextAsync(
        IServiceProvider services, IWebPushSender sender, TimeProvider clock, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        var processor = new PushDeliveryProcessor(
            scope.ServiceProvider.GetRequiredService<PushDeliveryQueue>(),
            scope.ServiceProvider.GetRequiredService<IPushSubscriptionRepository>(),
            sender, clock, NullLogger<PushDeliveryProcessor>.Instance);
        return await processor.ProcessNextAsync(ct);
    }

    private sealed class TestClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; }

        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class BlockingSender : IWebPushSender
    {
        public TaskCompletionSource<Guid> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<WebPushSendResult> SendAsync(
            Guid deliveryId, string payload, DateTimeOffset expiresAt, PushSubscription subscription, CancellationToken ct)
        {
            Entered.TrySetResult(deliveryId);
            await Release.Task.WaitAsync(ct);
            return new WebPushSendResult(WebPushSendStatus.Accepted, 201);
        }
    }

    private sealed class FakeSender(Func<PushSubscription, WebPushSendResult> send) : IWebPushSender
    {
        public int Calls { get; private set; }

        public Task<WebPushSendResult> SendAsync(
            Guid deliveryId, string payload, DateTimeOffset expiresAt, PushSubscription subscription, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(send(subscription));
        }
    }
}
