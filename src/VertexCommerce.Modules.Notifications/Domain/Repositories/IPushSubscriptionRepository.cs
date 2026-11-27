using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Domain.Repositories;

public interface IPushSubscriptionRepository
{
    Task<bool> TryUpsertOwnedAsync(PushSubscription subscription, CancellationToken ct = default);
    Task RemoveOwnedByEndpointAsync(Guid userId, string endpoint, CancellationToken ct = default);
    Task<PushSubscription?> GetByIdAsync(Guid subscriptionId, CancellationToken ct = default);
    Task<IReadOnlyList<PushSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task RemoveIfUnchangedAsync(PushSubscription snapshot, CancellationToken ct = default);
}
