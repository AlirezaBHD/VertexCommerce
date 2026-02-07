using VertexCommerce.Modules.Basket.Domain.Entities;

namespace VertexCommerce.Modules.Basket.Domain.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<CustomerBasket?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task CreateAsync(CustomerBasket basket, CancellationToken ct = default);
    Task UpdateAsync(CustomerBasket basket, CancellationToken ct = default);
    Task DeleteAsync(Guid customerId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid customerId, CancellationToken ct = default);
}
