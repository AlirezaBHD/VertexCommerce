using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Domain.Repositories;

public interface IShippingSettingsRepository
{
    Task<ShippingSettings?> GetActiveAsync(CancellationToken ct = default);
    Task<ShippingSettings?> GetAsync(CancellationToken ct = default);
    Task AddAsync(ShippingSettings settings, CancellationToken ct = default);
    void Update(ShippingSettings settings);
}
