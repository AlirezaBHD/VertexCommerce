using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
}
