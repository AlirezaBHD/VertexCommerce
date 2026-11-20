using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services.Models;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal sealed class NotificationDispatcher(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork,
    ISseConnectionManager sseManager,
    IWebPushService webPushService,
    IIdentityService identityService,
    ILogger<NotificationDispatcher> logger)
    : INotificationDispatcher
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Notification dispatch failure must not fail the primary business operation")]
    public async Task DispatchToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        string? payloadJson = null,
        string? pushUrl = null,
        object? pushData = null,
        CancellationToken ct = default)
    {
        try
        {
            var notification = Notification.Create(
                userId: userId,
                title: title,
                message: message,
                type: type,
                payloadJson: payloadJson);

            await notificationRepository.AddAsync(notification, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var sseMessage = new NotificationMessage(
                Id: notification.Id,
                UserId: notification.UserId,
                Title: notification.Title,
                Message: notification.Message,
                Type: notification.Type,
                PayloadJson: notification.PayloadJson,
                IsRead: notification.IsRead,
                CreatedAt: notification.CreatedAt);

            await sseManager.BroadcastToUserAsync(userId, sseMessage, ct);

            await webPushService.SendPushToUserAsync(
                userId: userId,
                title: title,
                body: message,
                url: pushUrl,
                data: pushData,
                ct: ct);

            logger.LogInformation("Notification {Id} ({Type}) dispatched to user {UserId}",
                notification.Id, type, userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to dispatch notification ({Type}) to user {UserId}", type, userId);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Admin notification dispatch failure must not fail the primary business operation")]
    public async Task DispatchToAdminsAsync(
        string title,
        string message,
        string type,
        string? payloadJson = null,
        string? pushUrl = null,
        object? pushData = null,
        CancellationToken ct = default)
    {
        try
        {
            var adminIds = await identityService.GetAdminUserIdsAsync(ct);
            if (adminIds.Count == 0)
            {
                return;
            }

            logger.LogInformation("Dispatching admin notification ({Type}) to {Count} admin(s)", type, adminIds.Count);

            var notifications = new List<Notification>(adminIds.Count);
            foreach (var adminId in adminIds)
            {
                notifications.Add(Notification.Create(
                    userId: adminId,
                    title: title,
                    message: message,
                    type: type,
                    payloadJson: payloadJson));
            }

            await notificationRepository.AddRangeAsync(notifications, ct);
            await unitOfWork.SaveChangesAsync(ct);

            foreach (var notification in notifications)
            {
                var sseMessage = new NotificationMessage(
                    Id: notification.Id,
                    UserId: notification.UserId,
                    Title: notification.Title,
                    Message: notification.Message,
                    Type: notification.Type,
                    PayloadJson: notification.PayloadJson,
                    IsRead: notification.IsRead,
                    CreatedAt: notification.CreatedAt);

                await sseManager.BroadcastToUserAsync(notification.UserId, sseMessage, ct);

                await webPushService.SendPushToUserAsync(
                    userId: notification.UserId,
                    title: title,
                    body: message,
                    url: pushUrl,
                    data: pushData,
                    ct: ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to dispatch admin notification ({Type})", type);
        }
    }
}
