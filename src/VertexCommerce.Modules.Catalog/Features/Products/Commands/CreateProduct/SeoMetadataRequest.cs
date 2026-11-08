namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record SeoMetadataRequest(
    string Slug,
    string MetaTitle,
    string MetaDescription,
    string? Keywords
);
