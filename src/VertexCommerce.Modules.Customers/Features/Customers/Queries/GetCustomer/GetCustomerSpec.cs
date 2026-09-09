using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

public sealed class GetCustomerSpec : BaseSpecification<Customer, CustomerResponse>
{
    public GetCustomerSpec(Guid userId)
    {
        Where(c => c.UserId == userId);

        Include(c => c.Addresses);
        Select(c => new CustomerResponse(
            Id: c.Id,
            UserId: c.UserId,
            PhoneNumber: c.PhoneNumber.Value,
            FirstName: c.FirstName.Value,
            LastName: c.LastName.Value,
            Addresses: MapAddresses(c.Addresses),
            DefaultShippingAddressId: c.DefaultShippingAddressId,
            DefaultBillingAddressId: c.DefaultBillingAddressId
        ));
    }

    private static List<AddressSummaryResponse> MapAddresses(IEnumerable<CustomerAddress> addresses) =>
        addresses.Select(AddressSummaryResponse.From).ToList();
}
