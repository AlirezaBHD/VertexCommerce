using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using NSubstitute;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Shared.IntegrationEvents;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class NotificationHttpContractTests
{
    private const string Endpoint = "/api/notifications";

    [Fact]
    public async Task Inbox_lists_the_requested_page_and_updates_read_state_for_the_current_user()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        await using var host = await NotificationsTestHost.StartAsync(ct);
        using var client = host.CreateClient();
        var userId = Guid.NewGuid();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(userId));

        const string payload = """{"OrderNumber":"VC-100"}""";
        var own = Notification.Create(userId, "Title", "Message", "OrderCreated", payload);
        var other = Notification.Create(Guid.NewGuid(), "Other title", "Other message", "OrderCreated");
        var repository = host.Dependencies.Notifications;
        repository.GetUserNotificationsAsync(userId, 1, 20, Arg.Any<CancellationToken>()).Returns([own]);
        repository.GetUserNotificationsAsync(userId, 2, 1, Arg.Any<CancellationToken>()).Returns([own]);
        repository.GetUnreadCountAsync(userId, Arg.Any<CancellationToken>()).Returns(_ => own.IsRead ? 0 : 1);
        repository.GetByIdAsync(own.Id, Arg.Any<CancellationToken>()).Returns(own);
        repository.GetByIdAsync(other.Id, Arg.Any<CancellationToken>()).Returns(other);

        var defaultPage = await client.GetFromJsonAsync<JsonElement>(Endpoint, ct);
        Assert.Single(defaultPage.EnumerateArray());
        var list = await client.GetFromJsonAsync<JsonElement>($"{Endpoint}?page=2&pageSize=1", ct);
        var item = Assert.Single(list.EnumerateArray());
        Assert.Equal(own.Id, item.GetProperty("id").GetGuid());
        Assert.Equal(userId, item.GetProperty("userId").GetGuid());
        Assert.Equal("Title", item.GetProperty("title").GetString());
        Assert.Equal("Message", item.GetProperty("message").GetString());
        Assert.Equal("OrderCreated", item.GetProperty("type").GetString());
        Assert.Equal(payload, item.GetProperty("payloadJson").GetString());
        Assert.False(item.GetProperty("isRead").GetBoolean());
        Assert.Equal(JsonValueKind.Null, item.GetProperty("readAt").ValueKind);
        Assert.Equal(own.CreatedAt, item.GetProperty("createdAt").GetDateTime());

        var unread = await client.GetFromJsonAsync<JsonElement>($"{Endpoint}/unread-count", ct);
        Assert.Equal(1, unread.GetProperty("count").GetInt32());

        using var forbiddenRead = await client.PatchAsync(Route($"{Endpoint}/{other.Id}/read"), null, ct);
        Assert.Equal(HttpStatusCode.NotFound, forbiddenRead.StatusCode);
        Assert.False(other.IsRead);
        await host.Dependencies.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        using var markRead = await client.PatchAsync(Route($"{Endpoint}/{own.Id}/read"), null, ct);
        Assert.Equal(HttpStatusCode.NoContent, markRead.StatusCode);
        Assert.True(own.IsRead);
        var firstReadAt = Assert.IsType<DateTime>(own.ReadAt);
        await host.Dependencies.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        using var repeatRead = await client.PatchAsync(Route($"{Endpoint}/{own.Id}/read"), null, ct);
        Assert.Equal(HttpStatusCode.NoContent, repeatRead.StatusCode);
        Assert.Equal(firstReadAt, own.ReadAt);

        unread = await client.GetFromJsonAsync<JsonElement>($"{Endpoint}/unread-count", ct);
        Assert.Equal(0, unread.GetProperty("count").GetInt32());

        using var markAllRead = await client.PatchAsync(Route($"{Endpoint}/read-all"), null, ct);
        Assert.Equal(HttpStatusCode.NoContent, markAllRead.StatusCode);
        await repository.Received(1).MarkAllAsReadAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Push_key_is_public_and_authenticated_clients_can_subscribe_and_unsubscribe()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        await using var host = await NotificationsTestHost.StartAsync(ct);
        using var client = host.CreateClient();

        var key = await client.GetFromJsonAsync<JsonElement>($"{Endpoint}/vapid-public-key", ct);
        Assert.Equal(host.VapidPublicKey, key.GetProperty("publicKey").GetString());

        const string pushEndpoint = TestPushSubscription.Endpoint;
        var request = TestPushSubscription.Create();
        host.Dependencies.Subscriptions.TryUpsertOwnedAsync(Arg.Any<PushSubscription>(), Arg.Any<CancellationToken>())
            .Returns(true);
        using var unauthorized = await client.PostAsJsonAsync($"{Endpoint}/push/subscribe", request, ct);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        var userId = Guid.NewGuid();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(userId));
        using var subscribe = await client.PostAsJsonAsync($"{Endpoint}/push/subscribe", request, ct);
        Assert.Equal(HttpStatusCode.OK, subscribe.StatusCode);
        var response = await subscribe.Content.ReadFromJsonAsync<JsonElement>(ct);
        Assert.True(response.GetProperty("success").GetBoolean());
        await host.Dependencies.Subscriptions.Received(1).TryUpsertOwnedAsync(
            Arg.Is<PushSubscription>(subscription =>
                subscription.UserId == userId && subscription.Endpoint == pushEndpoint &&
                subscription.P256dhKey == request.P256dh && subscription.AuthKey == request.Auth &&
                subscription.UserAgent == request.UserAgent),
            Arg.Any<CancellationToken>());
        using var unsubscribe = await client.PostAsJsonAsync($"{Endpoint}/push/unsubscribe", new { endpoint = pushEndpoint }, ct);
        Assert.Equal(HttpStatusCode.NoContent, unsubscribe.StatusCode);
        await host.Dependencies.Subscriptions.Received(1).RemoveOwnedByEndpointAsync(userId, pushEndpoint, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Disabled_push_reports_unavailable_for_key_and_registration()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        await using var host = await NotificationsTestHost.StartAsync(ct, webPushEnabled: false);
        using var client = host.CreateClient();

        using var key = await client.GetAsync(new Uri($"{Endpoint}/vapid-public-key", UriKind.Relative), ct);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, key.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(Guid.NewGuid()));
        using var register = await client.PostAsJsonAsync($"{Endpoint}/push/subscribe", TestPushSubscription.Create(), ct);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, register.StatusCode);
    }

    [Theory]
    [InlineData("endpoint", "https://untrusted.example.test/push")]
    [InlineData("endpoint", "http://fcm.googleapis.com/push")]
    [InlineData("endpoint", "https://fcm.googleapis.com:444/push")]
    [InlineData("p256dh", "not-a-public-key")]
    [InlineData("auth", "not-an-auth-secret")]
    [InlineData("userAgent", null)]
    public async Task Subscribe_rejects_invalid_input_before_accessing_storage(string field, string? value)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        await using var host = await NotificationsTestHost.StartAsync(ct);
        using var client = host.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.CreateToken(Guid.NewGuid()));

        var request = TestPushSubscription.Create();
        var invalid = field switch
        {
            "endpoint" => request with { Endpoint = value ?? string.Empty },
            "p256dh" => request with { P256dh = value ?? string.Empty },
            "auth" => request with { Auth = value ?? string.Empty },
            _ => request with { UserAgent = new string('x', 513) }
        };
        using var response = await client.PostAsJsonAsync($"{Endpoint}/push/subscribe", invalid, ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await host.Dependencies.Subscriptions.DidNotReceive()
            .TryUpsertOwnedAsync(Arg.Any<PushSubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Stream_uses_query_token_and_delivers_notifications_only_to_the_recipient()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(40));
        var ct = timeout.Token;
        await using var host = await NotificationsTestHost.StartAsync(ct);
        using var client = host.CreateClient();
        var recipient = Guid.NewGuid();
        var otherUser = Guid.NewGuid();

        using var unauthorized = await client.GetAsync(Route($"{Endpoint}/stream"), HttpCompletionOption.ResponseHeadersRead, ct);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        using (var first = await OpenStreamAsync(client, host.CreateToken(recipient), ct))
        using (var second = await OpenStreamAsync(client, host.CreateToken(recipient), ct))
        using (var other = await OpenStreamAsync(client, host.CreateToken(otherUser), ct))
        using (var firstReader = new StreamReader(await first.Content.ReadAsStreamAsync(ct)))
        using (var secondReader = new StreamReader(await second.Content.ReadAsStreamAsync(ct)))
        using (var otherReader = new StreamReader(await other.Content.ReadAsStreamAsync(ct)))
        {
            foreach (var reader in new[] { firstReader, secondReader, otherReader })
            {
                Assert.Equal("event: connected", await reader.ReadLineAsync(ct));
                Assert.Equal("data: {}", await reader.ReadLineAsync(ct));
                Assert.Equal(string.Empty, await reader.ReadLineAsync(ct));
            }

            Assert.Equal(3, host.Connections.ActiveConnectionsCount);

            var order = new OrderCreatedIntegrationEvent(Guid.NewGuid(), recipient, "VC-100", 1250, "IRR");
            await host.PublishAsync(order, ct);
            await host.PublishAsync(new OrderCreatedIntegrationEvent(Guid.NewGuid(), otherUser, "VC-200", 3000, "IRR"), ct);

            var firstMessage = await ReadNotificationAsync(firstReader, ct);
            Assert.Equal(firstMessage.GetRawText(), (await ReadNotificationAsync(secondReader, ct)).GetRawText());
            Assert.Equal(otherUser, (await ReadNotificationAsync(otherReader, ct)).GetProperty("userId").GetGuid());
            Assert.NotEqual(Guid.Empty, firstMessage.GetProperty("id").GetGuid());
            Assert.Equal(recipient, firstMessage.GetProperty("userId").GetGuid());
            Assert.Equal("OrderCreated", firstMessage.GetProperty("type").GetString());
            Assert.Equal("سفارش شما ثبت شد", firstMessage.GetProperty("title").GetString());
            Assert.Equal("سفارش جدید شما با شماره VC-100 با موفقیت ثبت شد.", firstMessage.GetProperty("message").GetString());
            Assert.Equal(JsonSerializer.Serialize(order), firstMessage.GetProperty("payloadJson").GetString());
            Assert.False(firstMessage.GetProperty("isRead").GetBoolean());
            Assert.Equal(DateTimeKind.Utc, firstMessage.GetProperty("createdAt").GetDateTime().Kind);
            Assert.False(firstMessage.TryGetProperty("readAt", out _));

            Assert.Equal(": ping", await firstReader.ReadLineAsync(ct));
            Assert.Equal(string.Empty, await firstReader.ReadLineAsync(ct));
        }

        using var cleanupTimeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cleanupTimeout.CancelAfter(TimeSpan.FromSeconds(5));
        while (host.Connections.ActiveConnectionsCount != 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(25), cleanupTimeout.Token);
        }
    }

    private static Uri Route(string path) => new(path, UriKind.Relative);

    private static async Task<HttpResponseMessage> OpenStreamAsync(HttpClient client, string token, CancellationToken ct)
    {
        var response = await client.GetAsync(
            Route($"{Endpoint}/stream?access_token={Uri.EscapeDataString(token)}"), HttpCompletionOption.ResponseHeadersRead, ct);
        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/event-stream", response.Content.Headers.ContentType?.MediaType);
            Assert.True(response.Headers.CacheControl?.NoCache);
            Assert.Equal("no", Assert.Single(response.Headers.GetValues("X-Accel-Buffering")));
            return response;
        }
        catch
        {
            response.Dispose();
            throw;
        }
    }

    private static async Task<JsonElement> ReadNotificationAsync(StreamReader reader, CancellationToken ct)
    {
        Assert.Equal("event: notification", await reader.ReadLineAsync(ct));
        var data = await reader.ReadLineAsync(ct);
        Assert.NotNull(data);
        Assert.StartsWith("data: ", data, StringComparison.Ordinal);
        Assert.Equal(string.Empty, await reader.ReadLineAsync(ct));
        using var json = JsonDocument.Parse(data.AsMemory("data: ".Length));
        return json.RootElement.Clone();
    }
}
