using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;

public sealed record AdminAddAddressCommand(
    Guid CustomerId,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
) : ICommand<AddressResponse>;
