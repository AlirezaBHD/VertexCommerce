using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

public sealed record AddAddressCommand(
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
) : ICommand<AddressSummaryResponse>;
