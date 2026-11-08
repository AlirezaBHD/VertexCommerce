using VertexCommerce.Modules.Orders.Domain.Enums;

namespace VertexCommerce.Modules.Orders.GraphQL.Orders.Types;

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
