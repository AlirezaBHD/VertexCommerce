using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    Dictionary<string, string>? Attributes,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto>? Variants
);


public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    Dictionary<string, string>? Attributes,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto> Variants
) : ICommand;

public sealed record UpdateVariantDto(
    Guid? Id,
    decimal Price,
    string? Currency,
    int StockQuantity,
    int Order,
    string? Sku,
    List<UpdateVariantOptionDto> Options,
    List<UpdateMediaDto> Medias
);

public sealed record UpdateMediaDto(
    string Path,
    int Order
);

public sealed record UpdateVariantOptionDto(
    string Name,
    string Value
);

public sealed record UpdateSeoMetadataDto(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);
