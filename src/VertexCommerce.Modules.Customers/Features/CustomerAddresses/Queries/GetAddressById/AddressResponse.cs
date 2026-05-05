namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed record AddressResponse(
    Guid Id,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
);