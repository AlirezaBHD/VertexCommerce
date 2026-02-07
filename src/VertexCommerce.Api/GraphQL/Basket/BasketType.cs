namespace VertexCommerce.Api.GraphQL.Basket;

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
