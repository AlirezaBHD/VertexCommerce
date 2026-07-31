using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string PhoneNumber,
    string FirstName,
    string LastName
) : ICommand<CreateCustomerResponse>;

public sealed record CreateCustomerResponse(
    Guid Id,
    string PhoneNumber,
    string FirstName,
    string LastName
);
