using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.GetCustomer;

public sealed record GetCustomerQuery(Guid UserId) : IQuery<CustomerResponse>;

public sealed record CustomerResponse(
    Guid Id,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? Phone,
    IReadOnlyList<AddressResponse> Addresses,
    Guid? DefaultShippingAddressId,
    Guid? DefaultBillingAddressId
);

public sealed record AddressResponse(
    Guid Id,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string? Label,
    string FullAddress
);
