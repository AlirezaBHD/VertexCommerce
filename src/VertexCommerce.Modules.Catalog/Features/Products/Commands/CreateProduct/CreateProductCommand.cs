using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    Guid CategoryId,
    SeoMetadataRequest SeoMetadata,
    List<CreateVariantRequest>? Variants,
    List<CreateMediaRequest>? Media
) : ICommand<CreateProductResponse>;

public sealed record SeoMetadataRequest(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);

public sealed record CreateVariantRequest(
    decimal Price,
    string? Currency,
    int StockQuantity,
    int SortOrder,
    List<ProductAttributeRequest> Attributes
);

public sealed record ProductAttributeRequest(
    string AttributeCode,
    string OptionCode
);

public sealed record CreateMediaRequest(
    string Path,
    int SortOrder,
    string? AltText,
    string? AssociatedAttributeCode,
    string? AssociatedOptionCode
);

public sealed record CreateProductResponse(
    Guid ProductId,
    List<VariantInfo> Variants
);

public sealed record VariantInfo(
    Guid VariantId,
    string Sku
);
