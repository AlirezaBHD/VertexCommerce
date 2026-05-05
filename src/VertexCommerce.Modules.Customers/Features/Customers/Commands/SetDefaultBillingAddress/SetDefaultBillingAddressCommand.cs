using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultBillingAddress;

public sealed record SetDefaultBillingAddressCommand(Guid AddressId) : ICommand;
