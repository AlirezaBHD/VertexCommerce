using MediatR;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerInfo;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Customers.Services;

internal sealed class CustomerService(ISender sender) 
    : ICustomerService
{
    public async Task<CustomerInfoDto?> GetCustomerInfo(Guid customerId,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCustomerInfoQuery(customerId), ct);
        return result.Value;
    }
}