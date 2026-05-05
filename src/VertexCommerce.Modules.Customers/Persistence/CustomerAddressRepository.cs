using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Persistence;

internal sealed class CustomerAddressRepository(CustomersDbContext context) : ICustomerAddressRepository
{
    public async Task AddAsync(CustomerAddress address, CancellationToken ct = default)
    {
        await context.CustomerAddresses.AddAsync(address, ct);
    }

    public async Task<CustomerAddress?> GetAsync(Guid customerId, Guid addressId, CancellationToken ct)
    {
        var query = context.CustomerAddresses
            .Where(a => a.CustomerId == customerId && a.Id == addressId);
        
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<TResult?> GetAsync<TResult>(ISpecification<CustomerAddress, TResult> spec,
        CancellationToken ct = default)
    {
        var query = context.CustomerAddresses
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec)
            .FirstOrDefaultAsync(ct);
    }
    
}
