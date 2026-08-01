using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerAdmin;

public sealed record GetCustomerAdminQuery(Guid CustomerId)
    : IQuery<CustomerAdminDetailResponse>;

public sealed record CustomerAdminDetailResponse(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName,
    string FullName,
    Guid? DefaultShippingAddressId,
    Guid? DefaultBillingAddressId,
    IReadOnlyList<AddressResponse> Addresses,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
