using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts.Identity;

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

    private static IResult GetVapidPublicKey([FromServices] IWebPushService webPushService)
    {
        return Results.Ok(new { publicKey = webPushService.PublicKey });
    }

    private static async Task<IResult> GetNotifications(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] ICurrentUser currentUser,
        [FromServices] INotificationRepository repository,
        CancellationToken ct)
    {
        var pageNumber = page.GetValueOrDefault(1);
        var size = pageSize.GetValueOrDefault(20);
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        size = size <= 0 ? 20 : Math.Min(size, 50);

        var notifications = await repository.GetUserNotificationsAsync(currentUser.UserId, pageNumber, size, ct);
        return Results.Ok(notifications);
    }

    private static async Task<IResult> GetUnreadCount(
        [FromServices] ICurrentUser currentUser,
        [FromServices] INotificationRepository repository,
        CancellationToken ct)
    {
        var count = await repository.GetUnreadCountAsync(currentUser.UserId, ct);
        return Results.Ok(new { count });
    }

    private static async Task StreamNotifications(
        HttpContext context,
        [FromServices] ICurrentUser currentUser,
        [FromServices] ISseConnectionManager sseManager,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;

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
                    // Send keep-alive comment
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
        [FromServices] ICurrentUser currentUser,
        [FromServices] IPushSubscriptionRepository repository,
        [FromServices] INotificationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Endpoint) ||
            string.IsNullOrWhiteSpace(request.P256dh) ||
            string.IsNullOrWhiteSpace(request.Auth))
        {
            return Results.BadRequest(new { error = "Invalid subscription keys" });
        }

        var subscription = PushSubscription.Create(
            userId: currentUser.UserId,
            endpoint: request.Endpoint,
            p256DhKey: request.P256dh,
            authKey: request.Auth,
            userAgent: request.UserAgent);

        await repository.AddOrUpdateAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Results.Ok(new { success = true });
    }

    private static async Task<IResult> UnsubscribeFromPush(
        [FromBody] PushUnsubscribeRequest request,
        [FromServices] IPushSubscriptionRepository repository,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Endpoint))
        {
            await repository.RemoveByEndpointAsync(request.Endpoint, ct);
        }

        return Results.NoContent();
    }

    private static async Task<IResult> MarkAsRead(
        Guid id,
        [FromServices] ICurrentUser currentUser,
        [FromServices] INotificationRepository repository,
        [FromServices] INotificationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);
        if (notification is null || notification.UserId != currentUser.UserId)
        {
            return Results.NotFound();
        }

        notification.MarkAsRead();
        await unitOfWork.SaveChangesAsync(ct);

        return Results.NoContent();
    }

    private static async Task<IResult> MarkAllAsRead(
        [FromServices] ICurrentUser currentUser,
        [FromServices] INotificationRepository repository,
        CancellationToken ct)
    {
        await repository.MarkAllAsReadAsync(currentUser.UserId, ct);
        return Results.NoContent();
    }
}

public sealed record PushSubscribeRequest(
    string Endpoint,
    string P256dh,
    string Auth,
    string? UserAgent
);

public sealed record PushUnsubscribeRequest(
    string Endpoint
);
