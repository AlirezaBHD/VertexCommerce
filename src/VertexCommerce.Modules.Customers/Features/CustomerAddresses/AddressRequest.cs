namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses;

public sealed record AddressRequest(
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label
) : IAddressInput;