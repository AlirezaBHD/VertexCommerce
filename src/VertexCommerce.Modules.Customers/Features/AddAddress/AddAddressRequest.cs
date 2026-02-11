namespace VertexCommerce.Modules.Customers.Features.AddAddress;

public sealed record AddAddressRequest(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string? Label
);