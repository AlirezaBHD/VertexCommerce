namespace VertexCommerce.Api.GraphQL.Catalog;

public sealed class ProductType
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public string Sku { get; init; } = default!;
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
