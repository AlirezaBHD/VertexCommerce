using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;

namespace VertexCommerce.Modules.Notifications.Persistence.Repositories;

internal sealed class NotificationRepository(NotificationsDbContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification, CancellationToken ct = default)
    {
        await context.Notifications.AddAsync(notification, ct);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);
    }

    public async Task<bool> ExistsForEventAsync(
        Guid userId,
        string type,
        string orderNumber,
        string newStatus,
        CancellationToken ct = default)
    {
        var searchPattern = $"\"OrderNumber\":\"{orderNumber}\"";
        var statusPattern = $"\"NewStatus\":\"{newStatus}\"";

        return await context.Notifications
            .AsNoTracking()
            .AnyAsync(n => n.UserId == userId &&
                           n.Type == type &&
                           n.PayloadJson != null &&
                           n.PayloadJson.Contains(searchPattern) &&
                           n.PayloadJson.Contains(statusPattern), ct);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, now), ct);
    }
}
