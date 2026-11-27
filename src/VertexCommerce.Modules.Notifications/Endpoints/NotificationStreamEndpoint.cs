using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Notifications.Endpoints;

internal static class NotificationStreamEndpoint
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(20);

    public static async Task StreamAsync(
        HttpContext context,
        [FromServices] ICurrentUser currentUser,
        [FromServices] ISseConnectionManager connections,
        CancellationToken ct)
    {
        var userId = currentUser.GetUserId();

        context.Response.Headers.Append("Content-Type", "text/event-stream");
        context.Response.Headers.Append("Cache-Control", "no-cache");
        context.Response.Headers.Append("Connection", "keep-alive");
        context.Response.Headers.Append("X-Accel-Buffering", "no");

        var channel = connections.RegisterClient(userId);

        try
        {
            await context.Response.WriteAsync("event: connected\ndata: {}\n\n", ct);
            await context.Response.Body.FlushAsync(ct);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, context.RequestAborted);

            while (!linkedCts.Token.IsCancellationRequested)
            {
                using var heartbeatCts = new CancellationTokenSource(HeartbeatInterval);
                using var readCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token, heartbeatCts.Token);

                try
                {
                    if (await channel.Reader.WaitToReadAsync(readCts.Token))
                    {
                        while (channel.Reader.TryRead(out var message))
                        {
                            var json = JsonSerializer.Serialize(message, JsonOptions);
                            await context.Response.WriteAsync($"event: notification\ndata: {json}\n\n", linkedCts.Token);
                            await context.Response.Body.FlushAsync(linkedCts.Token);
                        }
                    }
                }
                catch (OperationCanceledException) when (heartbeatCts.IsCancellationRequested && !linkedCts.Token.IsCancellationRequested)
                {
                    await context.Response.WriteAsync(": ping\n\n", linkedCts.Token);
                    await context.Response.Body.FlushAsync(linkedCts.Token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Client closed connection cleanly
        }
        finally
        {
            connections.UnregisterClient(userId, channel);
        }
    }
}
