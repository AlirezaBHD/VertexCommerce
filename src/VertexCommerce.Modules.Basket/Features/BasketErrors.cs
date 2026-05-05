using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features;

internal static class BasketErrors
{
    public static Error CustomerNotFound(Guid userId) =>
        Error.NotFound("Basket.CustomerNotFound",
            $"No customer profile found for user '{userId}'");

    public static Error VariantNotFound(Guid productId, Guid variantId) =>
        Error.NotFound("Basket.VariantNotFound",
            $"Product variant '{variantId}' not found for product '{productId}'");

    public static Error InsufficientStock(string sku, int available, int requested) =>
        Error.Validation("Basket.InsufficientStock",
            $"Insufficient stock for '{sku}'. Available: {available}, Requested: {requested}");

    public static Error QuantityExceedsStock(string sku, int current, int adding, int available) =>
        Error.Validation("Basket.QuantityExceedsStock",
            $"Cannot add {adding} of '{sku}'. Already in basket: {current}, Available stock: {available}");

    public static Error MaxQuantityExceeded(int maxQuantity) =>
        Error.Validation("Basket.MaxQuantityExceeded",
            $"Maximum quantity per item is {maxQuantity}");

    public static Error BasketIsFull(int maxItems) =>
        Error.Validation("Basket.BasketIsFull",
            $"Basket cannot contain more than {maxItems} distinct items");
}
