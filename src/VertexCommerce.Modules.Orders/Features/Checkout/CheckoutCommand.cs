using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed record CheckoutCommand(
    string? Notes
) : ICommand<CheckoutResponse>;

public sealed record CheckoutResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency
);
