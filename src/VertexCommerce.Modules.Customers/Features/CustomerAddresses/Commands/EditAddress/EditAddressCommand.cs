using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;

public sealed record EditAddressCommand(
    Guid AddressId,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null) : ICommand;
