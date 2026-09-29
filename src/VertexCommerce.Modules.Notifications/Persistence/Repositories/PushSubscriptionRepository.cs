using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;

namespace VertexCommerce.Modules.Notifications.Persistence.Repositories;

internal sealed class PushSubscriptionRepository(NotificationsDbContext context) : IPushSubscriptionRepository
{
    public async Task AddOrUpdateAsync(PushSubscription subscription, CancellationToken ct = default)
    {
        var existing = await context.PushSubscriptions
            .FirstOrDefaultAsync(s => s.Endpoint == subscription.Endpoint, ct);

        if (existing is not null)
        {
            context.PushSubscriptions.Remove(existing);
        }

        await context.PushSubscriptions.AddAsync(subscription, ct);
    }

    public async Task RemoveByEndpointAsync(string endpoint, CancellationToken ct = default)
    {
        await context.PushSubscriptions
            .Where(s => s.Endpoint == endpoint)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<PushSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.PushSubscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task RemoveAsync(PushSubscription subscription, CancellationToken ct = default)
    {
        context.PushSubscriptions.Remove(subscription);
        await Task.CompletedTask;
    }
}
