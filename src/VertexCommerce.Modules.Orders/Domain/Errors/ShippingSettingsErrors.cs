using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Domain.Errors;

public static class ShippingSettingsErrors
{
    public static readonly Error NotFound =
        Error.NotFound("ShippingSettings", "Shipping settings not found.");

    public static readonly Error NegativeCost =
        Error.Validation("Shipping cost cannot be negative.");

    public static readonly Error InvalidFreeShippingThreshold =
        Error.Validation("Free shipping threshold must be greater than zero.");
}
