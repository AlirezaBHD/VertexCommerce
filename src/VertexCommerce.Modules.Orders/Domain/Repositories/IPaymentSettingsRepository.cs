using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Domain.Repositories;

public interface IPaymentSettingsRepository
{
    Task<PaymentSettings?> GetActiveAsync(CancellationToken ct = default);
    Task<PaymentSettings?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PaymentSettings>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(PaymentSettings settings, CancellationToken ct = default);
    void Update(PaymentSettings settings);
    void Delete(PaymentSettings settings);
}
