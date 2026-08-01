using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Services;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Persistence;

internal sealed class CustomerRepository(CustomersDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<TResult?> GetAsync<TResult>(ISpecification<Customer, TResult> spec,
        CancellationToken ct = default)
    {
        var query = context.Customers
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
    {
        var normalized = phoneNumber.Trim();
        return await context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.PhoneNumber == normalized, ct);
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string? searchTerm, int limit = 20,
        CancellationToken ct = default)
    {
        var query = context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(c =>
                c.PhoneNumber.Contains(term) ||
                c.FirstName.Contains(term) ||
                c.LastName.Contains(term));
        }

        return await query
            .Include(c => c.Addresses)
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<TResult>> GetPaginatedAsync<TResult>(ISpecification<Customer, TResult> spec,
        int skip = 0, int take = 10, CancellationToken ct = default)
    {
        var baseQuery = context.Customers.AsQueryable();

        var query = SpecificationEvaluator
            .ApplySpecification(baseQuery, spec);

        var count = await query.CountAsync(ct);
        var result = await query.Skip(skip).Take(take).ToListAsync(ct);

        return new PagedResult<TResult>(
            Items: result,
            HasNextPage: count > (skip + 1) * take,
            HasPreviousPage: skip > 0,
            TotalCount: count
        );
    }

    public async Task<Guid> GetIdByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var query = context.Customers.AsNoTracking()
            .Where(c => c.UserId == userId).Select(c => c.Id);

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Customers.AnyAsync(c => c.UserId == userId, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await context.Customers.AddAsync(customer, ct);
    }

    public void Update(Customer customer)
    {
        context.Customers.Update(customer);
    }

    public async Task<CustomerInfoDto?> GetCustomerInfoAsync(GetCustomerInfoSpec spec, CancellationToken ct)
    {
        var query = context.Customers
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec)
            .FirstOrDefaultAsync(ct);
    }
}
