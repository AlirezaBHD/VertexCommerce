namespace VertexCommerce.Modules.Notifications.EventHandlers;

internal static class OrderNotificationTypes
{
    public const string OrderCreated = "OrderCreated";
    public const string OrderStatusChanged = "OrderStatusChanged";
    public const string AdminOrderCreated = "AdminOrderCreated";
    public const string AdminPaymentReceiptSubmitted = "AdminPaymentReceiptSubmitted";
}
