namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed record AddressResponse(
    Guid Id,
    Guid CustomerId,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    DateTime CreatedAt,
    string? Label = null
);