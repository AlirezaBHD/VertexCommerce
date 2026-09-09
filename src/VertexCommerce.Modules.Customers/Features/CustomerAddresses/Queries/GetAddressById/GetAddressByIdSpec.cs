using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed class GetAddressByIdSpec : BaseSpecification<CustomerAddress, AddressResponse>
{
    public GetAddressByIdSpec(Guid addressId, Guid customerId)
    {
        Where(a => a.Id == addressId && a.CustomerId == customerId);

        Select(a => AddressResponse.From(a));
    }
}
