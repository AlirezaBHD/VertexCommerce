using VertexCommerce.Modules.Orders.Domain.Enums;

namespace VertexCommerce.Api.GraphQL.Orders.Types;

public sealed class OrderType
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = default!;
    public Guid CustomerId { get; init; }
    public string? CustomerEmail { get; init; }
    public OrderStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public decimal SubTotal { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public List<OrderItemType> Items { get; init; } = [];
}

public sealed class OrderItemType
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public string? ProductSku { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}
