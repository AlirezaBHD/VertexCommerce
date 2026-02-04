namespace VertexCommerce.Modules.Catalog.Features.GetProducts;

public sealed record ProductListItem(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt
);
