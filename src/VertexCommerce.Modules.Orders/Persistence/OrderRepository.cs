using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<TResult>> GetPaginatedAsync<TResult>(ISpecification<Order, TResult> spec,
        int skip = 0, int take = 10,
        CancellationToken ct = default)
    {
        var baseQuery = _context.Orders.AsQueryable();

        var query = SpecificationEvaluator
            .ApplySpecification(baseQuery, spec);

        var count = await query.CountAsync(ct);
        var result = await query.Skip(skip).Take(take).ToListAsync(ct);

        return new PagedResult<TResult>
        (
            Items: result,
            HasNextPage: count > skip * take,
            HasPreviousPage: skip * take - take > 0,
            TotalCount: count
        );
    }

    public async Task<TResult?> GetOrderByIdAsync<TResult>(ISpecification<Order, TResult> spec, CancellationToken ct = default)
    {
        var query = _context.Orders.AsQueryable();
        var order = await SpecificationEvaluator
            .ApplySpecification(query, spec).FirstOrDefaultAsync(ct);

        return order;
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id, ct);
    }

    public async Task AddAsync(Order entity, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(entity, ct);
    }

    public void Update(Order entity)
    {
        _context.Orders.Update(entity);
    }

    public void Delete(Order entity)
    {
        _context.Orders.Remove(entity);
    }
}
