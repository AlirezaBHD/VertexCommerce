namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

public sealed record ProductResponse(
    string Name,
    string? Description,
    bool IsActive,
    Guid CategoryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    SeoMetadataResponse SeoMetadata,
    List<ProductVariantDto> Variants,
    List<ProductMediaDto> Media
);