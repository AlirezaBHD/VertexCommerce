using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed class GetAddressByIdSpec : BaseSpecification<CustomerAddress, AddressResponse>
{
    public GetAddressByIdSpec(Guid addressId, Guid customerId)
    {
        Where(a => a.Id == addressId && a.CustomerId == customerId);
        
        Select(a => new AddressResponse(
            Id: a.Id,
            CustomerId: a.CustomerId,
            Province: a.Province,
            City: a.City,
            PostalAddress: a.PostalAddress,
            PostalCode: a.PostalCode,
            Latitude: a.Latitude,
            Longitude: a.Longitude,
            CreatedAt:a.CreatedAt,
            Label: a.Label
        ));
    }
}
