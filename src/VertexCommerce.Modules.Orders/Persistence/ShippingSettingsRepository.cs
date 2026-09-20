using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;

namespace VertexCommerce.Modules.Orders.Persistence;

internal sealed class ShippingSettingsRepository(OrdersDbContext context) : IShippingSettingsRepository
{
    public async Task<ShippingSettings?> GetActiveAsync(CancellationToken ct = default)
        => await context.ShippingSettings
            .Where(s => !s.IsDeleted && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<ShippingSettings?> GetAsync(CancellationToken ct = default)
        => await context.ShippingSettings
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(ShippingSettings settings, CancellationToken ct = default)
        => await context.ShippingSettings.AddAsync(settings, ct);

    public void Update(ShippingSettings settings) => context.ShippingSettings.Update(settings);
}
