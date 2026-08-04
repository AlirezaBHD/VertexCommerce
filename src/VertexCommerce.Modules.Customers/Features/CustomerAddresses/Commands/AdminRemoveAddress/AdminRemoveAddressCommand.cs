using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminRemoveAddress;

public sealed record AdminRemoveAddressCommand(
    Guid CustomerId,
    Guid AddressId
) : ICommand;
