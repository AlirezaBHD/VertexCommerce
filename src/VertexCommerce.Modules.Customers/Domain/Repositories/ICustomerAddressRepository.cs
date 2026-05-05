using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Domain.Repositories;

public interface ICustomerAddressRepository
{
    Task AddAsync(CustomerAddress address, CancellationToken ct = default);
    Task<CustomerAddress?> GetAsync(Guid addressId, Guid customerId, CancellationToken ct);
    Task<TResult?> GetAsync<TResult>(ISpecification<CustomerAddress, TResult> spec,
        CancellationToken ct = default);
}
