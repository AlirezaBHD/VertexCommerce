using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;

namespace VertexCommerce.Modules.Orders.Persistence;

internal sealed class PaymentSettingsRepository(OrdersDbContext context) : IPaymentSettingsRepository
{
    public async Task<PaymentSettings?> GetActiveAsync(CancellationToken ct = default)
        => await context.PaymentSettings.FirstOrDefaultAsync(p => p.IsActive, ct);

    public async Task<PaymentSettings?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.PaymentSettings.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<PaymentSettings>> GetAllAsync(CancellationToken ct = default)
        => await context.PaymentSettings.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(PaymentSettings settings, CancellationToken ct = default)
        => await context.PaymentSettings.AddAsync(settings, ct);

    public void Update(PaymentSettings settings) => context.PaymentSettings.Update(settings);

    public void Delete(PaymentSettings settings) => context.PaymentSettings.Remove(settings);
}
