namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed record GetOrderByIdResponse(
    Guid Id,
    Guid CustomerId,
    string? CustomerName,
    string CustomerPhoneNumber,
    string OrderNumber,
    string Status,
    string PaymentStatus,
    string SubTotal,
    string ShippingCost,
    string TotalAmount,
    string? ReceiptImagePath,
    string? TrackingNumber,
    string ShippingAddress,
    string? CancellationReason,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ConfirmedAt,
    DateTime? ProcessingAt,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    DateTime? ExpiresAt,
    IReadOnlyList<GetOrderByIdOrderItemResponse> Items
);