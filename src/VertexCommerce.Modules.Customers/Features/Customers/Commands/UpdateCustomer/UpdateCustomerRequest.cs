namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerRequest(
    string PhoneNumber,
    string FirstName,
    string LastName
);