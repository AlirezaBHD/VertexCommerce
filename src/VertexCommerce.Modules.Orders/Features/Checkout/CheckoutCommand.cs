using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed record CheckoutCommand(
    string? Notes
) : ICommand<CheckoutResponse>;
