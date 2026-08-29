using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.Persistence;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Domain.Repositories;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<TResult>> GetPaginatedAsync<TResult>(ISpecification<Order, TResult> spec, int skip = 0,
        int take = 10, CancellationToken ct = default);
    Task<TResult?> GetOrderByIdAsync<TResult>(ISpecification<Order, TResult> spec, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetExpiredOrdersAsync(DateTime before, CancellationToken ct = default);
}
