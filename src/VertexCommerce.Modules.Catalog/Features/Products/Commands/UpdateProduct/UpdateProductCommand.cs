using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto>? Variants,
    List<UpdateMediaDto>? Media
) : ICommand;
