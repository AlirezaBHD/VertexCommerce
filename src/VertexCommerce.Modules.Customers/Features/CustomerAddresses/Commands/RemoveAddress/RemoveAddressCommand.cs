using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.RemoveAddress;

public sealed record RemoveAddressCommand(Guid AddressId) : ICommand;
