using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Customers.Services;

internal sealed class CustomerService(ICustomerRepository repository) 
    : ICustomerService
{
    public async Task<CustomerInfoDto?> GetCustomerInfo(Guid customerId,
        CancellationToken ct = default)
    {
        var spec = new GetCustomerInfoSpec(customerId: customerId);
        return await repository.GetCustomerInfoAsync(spec,ct);
    }
}