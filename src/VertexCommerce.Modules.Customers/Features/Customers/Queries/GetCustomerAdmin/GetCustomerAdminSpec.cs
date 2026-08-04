using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerAdmin;

public sealed class GetCustomerAdminSpec : BaseSpecification<Customer, CustomerAdminDetailResponse>
{
    public GetCustomerAdminSpec(Guid customerId)
    {
        Where(c => c.Id == customerId);

        Include(c => c.Addresses);

        Select(c => new CustomerAdminDetailResponse(
            Id: c.Id,
            UserId: c.UserId,
            PhoneNumber: c.PhoneNumber,
            FirstName: c.FirstName,
            LastName: c.LastName,
            FullName: c.FullName,
            DefaultShippingAddressId: c.DefaultShippingAddressId,
            DefaultBillingAddressId: c.DefaultBillingAddressId,
            Addresses: c.Addresses
                .Select(a => new AddressResponse(
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
                ))
                .ToList(),
            CreatedAt: c.CreatedAt,
            UpdatedAt: c.UpdatedAt
        ));
    }
}
