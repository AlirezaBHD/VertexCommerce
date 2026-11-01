namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

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