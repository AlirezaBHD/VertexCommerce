namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed record MyOrderResponse(
    Guid Id,
    string OrderNumber,
    string CustomerPhoneNumber,
    string Status,
    string PaymentStatus,
    string? ReceiptImagePath,
    string? TransactionReference,
    string ShippingAddress,
    string BillingAddress,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Tax,
    decimal TotalAmount,
    string Currency,
    string? Notes,
    string? CancellationReason,
    string? TrackingNumber,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? ProcessingAt,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    DateTime? ExpiresAt,
    IReadOnlyList<OrderItemResponse> Items
);
