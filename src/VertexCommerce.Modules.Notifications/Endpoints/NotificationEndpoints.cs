using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Notifications.Features.Notifications.Commands.MarkAllAsRead;
using VertexCommerce.Modules.Notifications.Features.Notifications.Commands.MarkAsRead;
using VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetNotifications;
using VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetUnreadCount;
using VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;
using VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.UnsubscribeFromPush;
using VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Queries.GetVapidPublicKey;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Notifications.Endpoints;

public static class NotificationEndpoints
{
    private static readonly JsonSerializerOptions SseJsonOptions = new(JsonSerializerDefaults.Web);

    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications");

        // Public / VAPID key
        group.MapGet("/vapid-public-key", GetVapidPublicKey);

        // Protected endpoints
        var authGroup = group.MapGroup("/").RequireAuthorization();

        authGroup.MapGet("/", GetNotifications);
        authGroup.MapGet("/unread-count", GetUnreadCount);
        authGroup.MapGet("/stream", StreamNotifications);
        authGroup.MapPost("/push/subscribe", SubscribeToPush);
        authGroup.MapPost("/push/unsubscribe", UnsubscribeFromPush);
        authGroup.MapPatch("/{id:guid}/read", MarkAsRead);
        authGroup.MapPatch("/read-all", MarkAllAsRead);

        return app;
    }

    private static async Task<IResult> GetVapidPublicKey(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetVapidPublicKeyQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(new { publicKey = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetNotifications(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var pageNumber = page ?? 1;
        var size = pageSize ?? 20;

        var query = new GetNotificationsQuery(pageNumber, size);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetUnreadCount(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUnreadCountQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(new { count = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task StreamNotifications(
        HttpContext context,
        [FromServices] ICurrentUser currentUser,
        [FromServices] ISseConnectionManager sseManager,
        CancellationToken ct)
    {
        var userId = currentUser.GetUserId();

        context.Response.Headers.Append("Content-Type", "text/event-stream");
        context.Response.Headers.Append("Cache-Control", "no-cache");
        context.Response.Headers.Append("Connection", "keep-alive");
        context.Response.Headers.Append("X-Accel-Buffering", "no");

        var channel = sseManager.RegisterClient(userId);

        try
        {
            await context.Response.WriteAsync("event: connected\ndata: {}\n\n", ct);
            await context.Response.Body.FlushAsync(ct);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, context.RequestAborted);

            while (!linkedCts.Token.IsCancellationRequested)
            {
                using var heartbeatCts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                using var readCts = CancellationTokenSource.CreateLinkedTokenSource(linkedCts.Token, heartbeatCts.Token);

                try
                {
                    if (await channel.Reader.WaitToReadAsync(readCts.Token))
                    {
                        while (channel.Reader.TryRead(out var message))
                        {
                            var json = JsonSerializer.Serialize(message, SseJsonOptions);
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
            sseManager.UnregisterClient(userId, channel);
        }
    }

    private static async Task<IResult> SubscribeToPush(
        [FromBody] PushSubscribeRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new SubscribeToPushCommand(
            Endpoint: request.Endpoint,
            P256dh: request.P256dh,
            Auth: request.Auth,
            UserAgent: request.UserAgent);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { success = true })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UnsubscribeFromPush(
        [FromBody] PushUnsubscribeRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UnsubscribeFromPushCommand(request.Endpoint);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> MarkAsRead(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new MarkAsReadCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> MarkAllAsRead(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new MarkAllAsReadCommand(), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
}
