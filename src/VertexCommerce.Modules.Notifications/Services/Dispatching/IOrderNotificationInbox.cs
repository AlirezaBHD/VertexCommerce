namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal interface IOrderNotificationInbox
{
    Task<bool> TryStoreAsync(Guid eventId, IReadOnlyList<InboxNotification> notifications, CancellationToken ct);
}
