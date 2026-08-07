namespace VertexCommerce.Shared.Contracts.Customers;

public interface ICustomerService
{
    Task<CustomerInfoDto?> GetCustomerInfo(Guid customerId,
        CancellationToken ct = default);
}

public sealed record CustomerInfoDto(
    string PhoneNumber,
    string? FirstName,
    string? LastName,
    AddressDto? ShippingAddress,
    AddressDto? BillingAddress);
