namespace VertexCommerce.Shared.Contracts.Customers;

public sealed record CustomerInfoDto(
    string PhoneNumber,
    string? FirstName,
    string? LastName,
    AddressDto? ShippingAddress,
    AddressDto? BillingAddress,
    Guid? UserId = null);