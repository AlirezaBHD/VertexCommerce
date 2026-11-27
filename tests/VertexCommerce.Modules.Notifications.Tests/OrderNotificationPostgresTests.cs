using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Models;
using VertexCommerce.Shared.IntegrationEvents;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class OrderNotificationPostgresTests
{
    [NotificationsPostgresFact]
    public async Task Concurrent_replays_create_one_batch_but_a_distinct_event_with_the_same_status_is_stored()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections, postgresConnection: database.ConnectionString);
        var adminId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        host.Dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([adminId]);
        var first = NewPaymentReceipt(orderId, customerId);
        await SeedSubscriptionsAsync(host.Services, customerId, adminId, ct);

        await Task.WhenAll(Enumerable.Range(0, 12).Select(_ => host.PublishAsync(first, ct)));

        await using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        await AssertBatchAsync(context, customerId, adminId, 1, ct);
        Assert.Equal(first.EventId, Assert.Single(await EventIdsAsync(context, ct)));
        Assert.Equal(2, await context.Set<PushDelivery>().CountAsync(ct));
        await VerifyDeliveryCountAsync(connections, 2, ct);

        var second = NewPaymentReceipt(orderId, customerId);
        Assert.NotEqual(first.EventId, second.EventId);
        await host.PublishAsync(second, ct);
        await AssertBatchAsync(context, customerId, adminId, 2, ct);
        Assert.Equal(4, await context.Set<PushDelivery>().CountAsync(ct));
        Assert.Equal(new[] { first.EventId, second.EventId }.Order(), (await EventIdsAsync(context, ct)).Order());
        await VerifyDeliveryCountAsync(connections, 4, ct);
    }

    [NotificationsPostgresFact]
    public async Task A_failed_batch_rolls_back_its_receipt_and_all_notifications_and_can_be_reprocessed()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections, postgresConnection: database.ConnectionString);
        await using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var adminId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        host.Dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([adminId]);
        var receipt = NewPaymentReceipt(Guid.NewGuid(), customerId);
        await SeedSubscriptionsAsync(host.Services, customerId, adminId, ct);
        await context.Database.ExecuteSqlRawAsync(
            """ALTER TABLE notifications."Notifications" ADD CONSTRAINT "test_reject_admin" CHECK ("Type" <> 'AdminPaymentReceiptSubmitted');""",
            ct);

        await host.PublishAsync(receipt, ct);

        Assert.Empty(await context.Notifications.AsNoTracking().ToArrayAsync(ct));
        Assert.Empty(await EventIdsAsync(context, ct));
        Assert.False(await context.Set<PushDelivery>().AnyAsync(ct));
        await VerifyDeliveryCountAsync(connections, 0, ct);

        await context.Database.ExecuteSqlRawAsync(
            """ALTER TABLE notifications."Notifications" DROP CONSTRAINT "test_reject_admin";""", ct);
        await host.PublishAsync(receipt, ct);

        await AssertBatchAsync(context, customerId, adminId, 1, ct);
        Assert.Equal(receipt.EventId, Assert.Single(await EventIdsAsync(context, ct)));
        Assert.Equal(2, await context.Set<PushDelivery>().CountAsync(ct));
        await VerifyDeliveryCountAsync(connections, 2, ct);
    }

    [NotificationsPostgresFact]
    public async Task Applying_the_inbox_migration_preserves_legacy_notification_ids_and_read_state()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        var module = new NotificationsModule();
        var services = new ServiceCollection().AddLogging();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:NotificationsDb"] = database.ConnectionString
        }).Build();
        module.RegisterServices(services, configuration);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        await context.GetService<IMigrator>().MigrateAsync("20260920101810_Base properties", ct);

        var userId = Guid.NewGuid();
        const string payload = """{"OrderNumber":"VC-legacy","NewStatus":"PaymentUnderReview"}""";
        var read = Notification.Create(userId, "Existing title", "Existing body", "OrderStatusChanged", payload);
        read.MarkAsRead();
        var unread = Notification.Create(userId, "Unread title", "Unread body", "OrderCreated");
        context.Notifications.AddRange(read, unread);
        await context.SaveChangesAsync(ct);
        var before = await context.Notifications.AsNoTracking().SingleAsync(item => item.Id == read.Id, ct);

        await context.Database.ExecuteSqlRawAsync(
            """UPDATE notifications."__EFMigrationsHistory" SET "MigrationId" = '20260920120000_InitialNotifications' WHERE "MigrationId" = '20260920090000_InitialNotifications';""",
            ct);

        await module.InitializeAsync(provider, ct);

        Assert.False(context.Database.HasPendingModelChanges());
        Assert.Empty(await EventIdsAsync(context, ct));
        var existing = await context.Notifications.AsNoTracking().ToArrayAsync(ct);
        Assert.Equal(2, existing.Length);
        var savedRead = Assert.Single(existing, item => item.Id == read.Id);
        Assert.Equal(before.ReadAt, savedRead.ReadAt);
        Assert.True(savedRead.IsRead);
        Assert.Equal(payload, savedRead.PayloadJson);
        Assert.False(Assert.Single(existing, item => item.Id == unread.Id).IsRead);
    }

    [NotificationsPostgresFact]
    public async Task A_full_length_persian_reason_is_stored_without_truncating_event_json()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        var reason = new string('م', 500);
        var order = new OrderStatusChangedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), "VC-100",
            "PaymentPending", "Cancelled", 1000, "IRR", CancellationReason: reason);
        await host.PublishAsync(order, ct);

        await using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var stored = await context.Notifications.AsNoTracking().SingleAsync(ct);
        Assert.Contains(reason, stored.Message, StringComparison.Ordinal);
        using var payload = System.Text.Json.JsonDocument.Parse(Assert.IsType<string>(stored.PayloadJson));
        Assert.Equal(reason, payload.RootElement.GetProperty("CancellationReason").GetString());
    }

    private static async Task SeedSubscriptionsAsync(IServiceProvider services, Guid customerId, Guid adminId, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPushSubscriptionRepository>();
        var customer = TestPushSubscription.Create(TestPushSubscription.Endpoint + "-customer");
        var admin = TestPushSubscription.Create(TestPushSubscription.Endpoint + "-admin");
        Assert.True(await repository.TryUpsertOwnedAsync(
            PushSubscription.Create(customerId, customer.Endpoint, customer.P256dh, customer.Auth), ct));
        Assert.True(await repository.TryUpsertOwnedAsync(
            PushSubscription.Create(adminId, admin.Endpoint, admin.P256dh, admin.Auth), ct));
    }

    private static OrderStatusChangedIntegrationEvent NewPaymentReceipt(Guid orderId, Guid customerId)
    {
        return new OrderStatusChangedIntegrationEvent(
            orderId, customerId, "VC-100", "PaymentPending", "PaymentUnderReview", 1000, "IRR");
    }

    private static async Task<Guid[]> EventIdsAsync(NotificationsDbContext context, CancellationToken ct)
    {
        return await context.Set<ProcessedOrderEvent>().Select(orderEvent => orderEvent.EventId).ToArrayAsync(ct);
    }

    private static async Task AssertBatchAsync(
        NotificationsDbContext context, Guid customerId, Guid adminId, int expectedBatches, CancellationToken ct)
    {
        var notifications = await context.Notifications.AsNoTracking().ToArrayAsync(ct);
        Assert.Equal(expectedBatches * 2, notifications.Length);
        var byRecipient = notifications.ToLookup(item => item.UserId, item => item.Type);
        Assert.Equal(Enumerable.Repeat("OrderStatusChanged", expectedBatches), byRecipient[customerId]);
        Assert.Equal(Enumerable.Repeat("AdminPaymentReceiptSubmitted", expectedBatches), byRecipient[adminId]);
    }

    private static Task VerifyDeliveryCountAsync(ISseConnectionManager connections, int count, CancellationToken ct)
    {
        return connections.Received(count).BroadcastToUserAsync(Arg.Any<Guid>(), Arg.Any<NotificationMessage>(), ct);
    }
}
