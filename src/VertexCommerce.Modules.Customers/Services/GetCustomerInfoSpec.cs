using System.Reflection.Emit;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Services;

public sealed class GetCustomerInfoSpec : BaseSpecification<Customer, CustomerInfoDto>
{
    public GetCustomerInfoSpec(Guid customerId)
    {
        Where(c => c.Id == customerId);

        Include(p => p.Addresses);
        Select(c => new CustomerInfoDto(
            PhoneNumber: c.PhoneNumber,
            FirstName: c.FirstName,
            LastName: c.LastName,
            ShippingAddress: MapAddress(c.GetDefaultShippingAddress()),
            BillingAddress: MapAddress(c.GetDefaultBillingAddress()))
        );
    }

    private static AddressDto? MapAddress(CustomerAddress? address)
    {
        if (address == null)
        {
            return null;
        }

        return new AddressDto(Province: address.Province,
            City: address.City,
            PostalAddress: address.PostalAddress,
            PostalCode: address.PostalCode,
            Latitude: address.Latitude,
            Longitude: address.Longitude,
            Label: address.Label);
    }
}
