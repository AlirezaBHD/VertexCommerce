namespace VertexCommerce.Shared.Contracts.Customers;

public sealed record AddressDto(
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label
);