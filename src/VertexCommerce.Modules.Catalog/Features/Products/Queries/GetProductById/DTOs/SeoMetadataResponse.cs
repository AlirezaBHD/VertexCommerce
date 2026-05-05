namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

public sealed record SeoMetadataResponse(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);