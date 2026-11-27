using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Endpoints;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class PushSubscriptionPostgresTests
{
    private const string SubscribePath = "/api/notifications/push/subscribe";
    private const string UnsubscribePath = "/api/notifications/push/unsubscribe";

    [NotificationsPostgresFact]
    public async Task Concurrent_registrations_respect_ownership_and_only_the_owner_can_remove_the_subscription()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var ct = timeout.Token;
        await using var database = await NotificationsPostgresDatabase.CreateAsync(ct);
        await using var host = await NotificationsTestHost.StartAsync(ct, postgresConnection: database.ConnectionString);
        using var clientA = host.CreateClient();
        using var clientB = host.CreateClient();
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        clientA.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(userA));
        clientB.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(userB));

        var first = TestPushSubscription.Create(userAgent: null);
        var other = TestPushSubscription.Create();
        var contested = await Task.WhenAll(SubscribeAsync(clientA, first, ct), SubscribeAsync(clientB, other, ct));
        var aWon = contested[0] == HttpStatusCode.OK;
        Assert.Equal(aWon ? HttpStatusCode.Conflict : HttpStatusCode.OK, contested[1]);
        Assert.Equal(aWon ? HttpStatusCode.OK : HttpStatusCode.Conflict, contested[0]);
        var winner = aWon ? clientA : clientB;
        var loser = aWon ? clientB : clientA;
        var ownerId = aWon ? userA : userB;
        var otherId = aWon ? userB : userA;

        await using var scope = host.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPushSubscriptionRepository>();
        var original = Assert.Single(await repository.GetByUserIdAsync(ownerId, ct));
        var updated = TestPushSubscription.Create(userAgent: "updated-agent");
        var retries = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => SubscribeAsync(winner, updated, ct)));
        Assert.All(retries, status => Assert.Equal(HttpStatusCode.OK, status));
        Assert.Empty(await repository.GetByUserIdAsync(otherId, ct));

        await repository.RemoveIfUnchangedAsync(original, ct);
        var saved = Assert.Single(await repository.GetByUserIdAsync(ownerId, ct));
        Assert.Equal(original.Id, saved.Id);
        Assert.Equal(updated.UserAgent, saved.UserAgent);
        Assert.Equal(updated.P256dh, saved.P256dhKey);
        Assert.Equal(updated.Auth, saved.AuthKey);

        Assert.Equal(HttpStatusCode.NoContent, await UnsubscribeAsync(loser, ct));
        Assert.Single(await repository.GetByUserIdAsync(ownerId, ct));
        Assert.Equal(HttpStatusCode.NoContent, await UnsubscribeAsync(winner, ct));
        Assert.Equal(HttpStatusCode.NoContent, await UnsubscribeAsync(winner, ct));
        Assert.Empty(await repository.GetByUserIdAsync(ownerId, ct));
    }

    private static async Task<HttpStatusCode> SubscribeAsync(HttpClient client, PushSubscribeRequest request, CancellationToken ct)
    {
        using var response = await client.PostAsJsonAsync(SubscribePath, request, ct);
        return response.StatusCode;
    }

    private static async Task<HttpStatusCode> UnsubscribeAsync(HttpClient client, CancellationToken ct)
    {
        using var response = await client.PostAsJsonAsync(UnsubscribePath, new { endpoint = TestPushSubscription.Endpoint }, ct);
        return response.StatusCode;
    }
}
