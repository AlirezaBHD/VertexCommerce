using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed record CheckoutCommand(
    Guid CustomerId,
    string? CustomerEmail,
    CheckoutAddressDto ShippingAddress,
    CheckoutAddressDto? BillingAddress,
    string? Notes
) : ICommand<CheckoutResponse>;

public sealed record CheckoutAddressDto(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);

public sealed record CheckoutResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency
);
