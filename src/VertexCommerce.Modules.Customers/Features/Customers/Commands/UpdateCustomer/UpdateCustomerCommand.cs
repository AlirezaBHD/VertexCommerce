using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string PhoneNumber,
    string FirstName,
    string LastName
) : ICommand<UpdateCustomerResponse>;

