using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto>? Variants,
    List<UpdateMediaDto>? Media
);


public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto>? Variants,
    List<UpdateMediaDto>? Media
) : ICommand;

public sealed record UpdateVariantDto(
    Guid? Id,
    decimal Price,
    string? Currency,
    int StockQuantity,
    int SortOrder,
    string? Sku,
    List<UpdateProductAttributeDto> Attributes
);

public sealed record UpdateProductAttributeDto(
    string AttributeCode,
    string OptionCode
);

public sealed record UpdateMediaDto(
    string Path,
    int SortOrder,
    string? AltText,
    string? AssociatedAttributeCode,
    string? AssociatedOptionCode
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
