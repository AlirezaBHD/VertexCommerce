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
            PhoneNumber: c.PhoneNumber.Value,
            FirstName: c.FirstName.Value,
            LastName: c.LastName.Value,
            FullName: c.FullName,
            DefaultShippingAddressId: c.DefaultShippingAddressId,
            DefaultBillingAddressId: c.DefaultBillingAddressId,
            Addresses: MapAddresses(c.Addresses),
            CreatedAt: c.CreatedAt,
            UpdatedAt: c.UpdatedAt
        ));
    }

    private static List<AddressResponse> MapAddresses(IEnumerable<CustomerAddress> addresses) =>
        addresses.Select(AddressResponse.From).ToList();
}
