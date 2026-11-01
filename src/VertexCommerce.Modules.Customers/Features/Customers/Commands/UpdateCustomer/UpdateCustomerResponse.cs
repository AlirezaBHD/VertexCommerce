namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerResponse(
    Guid Id,
    string PhoneNumber,
    string FirstName,
    string LastName
);
