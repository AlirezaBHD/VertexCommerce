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
