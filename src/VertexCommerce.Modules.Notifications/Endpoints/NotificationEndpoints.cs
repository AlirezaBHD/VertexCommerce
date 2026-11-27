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
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Notifications.Endpoints;

public static class NotificationEndpoints
{
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
        authGroup.MapGet("/stream", NotificationStreamEndpoint.StreamAsync);
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
            : ToPushError(result.Error);
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
            : ToPushError(result.Error);
    }

    private static IResult ToPushError(Error error)
    {
        return error == WebPushErrors.Unavailable
            ? Results.Problem(detail: error.Message, statusCode: StatusCodes.Status503ServiceUnavailable)
            : error.ToHttpResult();
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
