using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultShippingAddress;

public sealed record SetDefaultShippingAddressCommand(Guid AddressId) : ICommand;
