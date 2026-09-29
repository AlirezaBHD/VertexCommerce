namespace VertexCommerce.Modules.Notifications.Services;

public interface IWebPushService
{
    string PublicKey { get; }
    Task SendPushToUserAsync(
        Guid userId,
        string title,
        string body,
        string? url = null,
        object? data = null,
        CancellationToken ct = default);
}
