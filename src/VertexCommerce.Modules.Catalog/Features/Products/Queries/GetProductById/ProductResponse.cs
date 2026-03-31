namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed record ProductResponse(
    string Name,
    string? Description,
    bool IsActive,
    Guid CategoryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<ProductAttributeResponse> Attributes,
    SeoMetadataResponse  SeoMetadata,
    List<ProductVariantResponse>? Variants
        );

public sealed record ProductAttributeResponse(
    string Key,
    string Value,
    string? Type
);

public sealed record SeoMetadataResponse(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);

public sealed record ProductVariantResponse(
    Guid Id,
    decimal Price,
    int StockQuantity,
    int Order,
    List<VariantOptionDto> Options,
    List<MediaDto> Medias
);

public sealed record MediaDto(
    string Path,
    int Order
);

public sealed record VariantOptionDto(
    string Name,
    string Value
);