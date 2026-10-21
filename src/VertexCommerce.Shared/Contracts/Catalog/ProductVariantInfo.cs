namespace VertexCommerce.Shared.Contracts.Catalog;

public sealed record ProductVariantInfo(
    Guid ProductId,
    Guid VariantId,
    string Name,
    string Sku,
    string? ImagePath,
    decimal Price,
    int StockQuantity,
    List<ProductAttributeInfo> Attributes
);