namespace VertexCommerce.Modules.Catalog.Features.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId
);