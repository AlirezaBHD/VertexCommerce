using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    Guid CategoryId,
    Dictionary<string, string>? Attributes,
    List<CreateVariantDto>? Variants
) : ICommand<CreateProductResponse>;

public sealed record CreateVariantDto(
    decimal Price,
    string? Currency,
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


public sealed record CreateProductResponse(
    Guid ProductId,
    List<VariantInfo> Variants
);

public sealed record VariantInfo(
    Guid VariantId,
    string Sku
);