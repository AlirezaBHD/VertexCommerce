namespace VertexCommerce.Api.GraphQL.Basket.Types;

public sealed class BasketType
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Currency { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public int TotalItems { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public List<BasketItemType> Items { get; init; } = [];
}
