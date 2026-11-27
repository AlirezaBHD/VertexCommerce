namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal interface INotificationDispatcher
{
    Task DispatchOrderEventAsync(
        Guid eventId, Guid customerUserId, NotificationDraft customerDraft, NotificationDraft? adminDraft, CancellationToken ct);
}
