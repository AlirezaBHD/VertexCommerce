using VertexCommerce.Modules.Orders.Domain.ValueObjects;

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
    IReadOnlyList<GetOrderByIdOrderItemResponse> Items
);

public sealed record GetOrderByIdOrderItemResponse(
    Guid Id,
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string? ProductSku,
    Money UnitPrice,
    int Quantity,
    Money TotalPrice
);