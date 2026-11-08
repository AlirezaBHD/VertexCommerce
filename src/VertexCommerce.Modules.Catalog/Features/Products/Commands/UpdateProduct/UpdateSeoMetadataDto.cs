namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateSeoMetadataDto(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);
