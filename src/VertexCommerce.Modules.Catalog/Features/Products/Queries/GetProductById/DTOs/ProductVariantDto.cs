namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

public sealed record ProductVariantDto(
    Guid Id,
    string Sku,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    int SortOrder,
    List<ProductAttributeDto> Attributes
);