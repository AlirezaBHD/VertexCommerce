namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerResponse(
    Guid Id,
    string PhoneNumber,
    string FirstName,
    string LastName
);