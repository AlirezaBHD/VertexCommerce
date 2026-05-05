namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

public sealed record AddAddressRequest(
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
);