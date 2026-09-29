using System.Threading.Channels;
using VertexCommerce.Modules.Notifications.Services.Models;

namespace VertexCommerce.Modules.Notifications.Services;

public interface ISseConnectionManager
{
    Channel<NotificationMessage> RegisterClient(Guid userId);
    void UnregisterClient(Guid userId, Channel<NotificationMessage> channel);
    Task BroadcastToUserAsync(Guid userId, NotificationMessage message, CancellationToken ct = default);
    int ActiveConnectionsCount { get; }
}
