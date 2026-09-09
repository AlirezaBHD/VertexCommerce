using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Domain.Errors;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) =>
        Error.NotFound("Order", orderId);

    public static readonly Error EmptyBasket =
        Error.Validation("Basket is empty. Cannot checkout.");

    public static readonly Error CustomerNotFound =
        Error.NotFound("Customer", "Customer not found.");

    public static Error ProductVariantNotFound(string variantId) =>
        Error.NotFound("ProductVariant", variantId);

    public static readonly Error EmptyItems =
        Error.Validation("Order has no valid items.");

    public static readonly Error ShippingAddressNotSet =
        Error.Validation("Shipping address is not set. Cannot checkout.");

    public static readonly Error BillingAddressNotSet =
        Error.Validation("Billing address is not set. Cannot checkout.");

    public static Error NotFoundForCustomer(Guid orderId) =>
        Error.NotFound("Order for Customer", orderId);

    public static readonly Error PaymentExpired =
        Error.Validation("Payment time expired");

    public static Error InsufficientStock(string name, string sku, int requested, int available) => 
        Error.Validation($"Insufficient stock for '{name}' ({sku}). Requested: {requested}, Available: {available}.");
}
