using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

public sealed record GetCustomerQuery() : IQuery<CustomerResponse>;

public sealed record CustomerResponse(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName,
    IReadOnlyList<AddressSummaryResponse> Addresses,
    Guid? DefaultShippingAddressId,
    Guid? DefaultBillingAddressId
);

public sealed record AddressSummaryResponse(
    Guid Id,
    string Province,
    string City,
    string PostalAddress,
    string? Label = null
);
