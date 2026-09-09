using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerInfo;

public sealed class GetCustomerInfoSpec : BaseSpecification<Customer, CustomerInfoDto>
{
    public GetCustomerInfoSpec(Guid customerId)
    {
        Where(c => c.Id == customerId);

        Include(p => p.Addresses);
        Select(c => new CustomerInfoDto(
            PhoneNumber: c.PhoneNumber.Value,
            FirstName: c.FirstName.Value,
            LastName: c.LastName.Value,
            ShippingAddress: MapAddress(c.GetDefaultShippingAddress()),
            BillingAddress: MapAddress(c.GetDefaultBillingAddress()))
        );
    }

    private static AddressDto? MapAddress(CustomerAddress? address)
    {
        if (address is null)
        {
            return null;
        }

        return new AddressDto(
            Province: address.Address.Province.Value,
            City: address.Address.City.Value,
            PostalAddress: address.Address.PostalAddress.Value,
            PostalCode: address.Address.PostalCode.Value,
            Latitude: address.Address.Location.Latitude,
            Longitude: address.Address.Location.Longitude,
            Label: address.Label?.Value);
    }
}
