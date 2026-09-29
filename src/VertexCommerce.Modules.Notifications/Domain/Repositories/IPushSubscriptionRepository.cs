using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Domain.Repositories;

public interface IPushSubscriptionRepository
{
    Task AddOrUpdateAsync(PushSubscription subscription, CancellationToken ct = default);
    Task RemoveByEndpointAsync(string endpoint, CancellationToken ct = default);
    Task<IReadOnlyList<PushSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task RemoveAsync(PushSubscription subscription, CancellationToken ct = default);
}
