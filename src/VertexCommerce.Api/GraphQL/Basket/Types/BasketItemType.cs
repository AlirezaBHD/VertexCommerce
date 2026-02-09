namespace VertexCommerce.Api.GraphQL.Basket.Types;

public sealed class BasketItemType
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public string? ProductSku { get; init; }
    public string? ImageUrl { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
    public DateTime AddedAt { get; init; }
}
