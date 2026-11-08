namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    UpdateSeoMetadataDto SeoMetadata,
    List<UpdateVariantDto>? Variants,
    List<UpdateMediaDto>? Media
);
