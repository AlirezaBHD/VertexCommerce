namespace VertexCommerce.Modules.Orders.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    AwaitingPayment = 2,
    PaymentUnderReview = 3,
    Confirmed = 4,
    Processing = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8,
    Refunded = 9
}
