using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;

public sealed record AdminEditAddressCommand(
    Guid CustomerId,
    Guid AddressId,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
) : ICommand;
