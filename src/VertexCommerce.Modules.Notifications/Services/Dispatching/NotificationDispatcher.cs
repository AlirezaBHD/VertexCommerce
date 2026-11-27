using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Services.Models;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal sealed class NotificationDispatcher(
    IOrderNotificationInbox inbox,
    ISseConnectionManager connections,
    IIdentityService identityService,
    ILogger<NotificationDispatcher> logger)
    : INotificationDispatcher
{
    public async Task DispatchOrderEventAsync(
        Guid eventId, Guid customerUserId, NotificationDraft customerDraft, NotificationDraft? adminDraft, CancellationToken ct)
    {
        var notifications = new List<InboxNotification> { CreateNotification(customerUserId, customerDraft) };

        if (adminDraft is not null)
        {
            var adminIds = await identityService.GetAdminUserIdsAsync(ct);
            foreach (var adminId in adminIds.Distinct())
            {
                notifications.Add(CreateNotification(adminId, adminDraft));
            }
        }

        if (!await inbox.TryStoreAsync(eventId, notifications, ct))
        {
            logger.LogDebug("Order event {EventId} already has stored notifications", eventId);
            return;
        }

        foreach (var delivery in notifications)
        {
            await BroadcastAsync(eventId, delivery.Notification, ct);
        }
    }

    private static InboxNotification CreateNotification(Guid userId, NotificationDraft draft)
    {
        var notification = Notification.Create(userId, draft.Title, draft.Message, draft.Type, draft.PayloadJson);
        var payload = JsonSerializer.Serialize(new
        {
            userId,
            title = draft.Title,
            body = draft.Message,
            icon = "/logo.ico",
            badge = "/logo.ico",
            url = draft.PushUrl ?? "/account",
            data = draft.PushData
        });
        return new InboxNotification(notification, payload);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Live delivery failures are isolated to the recipient after the inbox is committed.")]
    private async Task BroadcastAsync(Guid eventId, Notification notification, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            var streamMessage = new NotificationMessage(
                notification.Id,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.PayloadJson,
                notification.IsRead,
                notification.CreatedAt);

            await connections.BroadcastToUserAsync(notification.UserId, streamMessage, ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Live delivery failed for notification {NotificationId} from order event {EventId}",
                notification.Id, eventId);
        }
    }
}
