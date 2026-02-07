using VertexCommerce.Modules.Orders.Domain.Enums;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed record OrderResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string? CustomerEmail,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    AddressResponse ShippingAddress,
    AddressResponse? BillingAddress,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Tax,
    decimal TotalAmount,
    string Currency,
    string? Notes,
    DateTime CreatedAt,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    IReadOnlyList<OrderItemResponse> Items
);

public sealed record AddressResponse(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductSku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);
