using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminSetDefaultAddress;

public sealed record AdminSetDefaultAddressCommand(
    Guid CustomerId,
    Guid AddressId
) : ICommand;
