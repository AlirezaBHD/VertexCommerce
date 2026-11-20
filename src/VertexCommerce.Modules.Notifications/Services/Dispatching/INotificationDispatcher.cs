namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

public interface INotificationDispatcher
{
    Task DispatchToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        string? payloadJson = null,
        string? pushUrl = null,
        object? pushData = null,
        CancellationToken ct = default);

    Task DispatchToAdminsAsync(
        string title,
        string message,
        string type,
        string? payloadJson = null,
        string? pushUrl = null,
        object? pushData = null,
        CancellationToken ct = default);
}
