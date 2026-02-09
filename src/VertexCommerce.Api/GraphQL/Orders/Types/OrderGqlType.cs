using VertexCommerce.Modules.Orders.Domain.Enums;

namespace VertexCommerce.Api.GraphQL.Orders.Types;

public sealed class OrderGql
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = default!;
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
}
