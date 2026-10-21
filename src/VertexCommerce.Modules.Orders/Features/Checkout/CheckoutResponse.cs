namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed record CheckoutResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency,
    DateTime? ExpiresAt
);