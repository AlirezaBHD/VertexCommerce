using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Contract;

public interface IBasketRepository
{
    Task<BasketDocument?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task UpsertAsync(BasketDocument basket, CancellationToken ct = default);
    Task DeleteAsync(Guid customerId, CancellationToken ct = default);
}
