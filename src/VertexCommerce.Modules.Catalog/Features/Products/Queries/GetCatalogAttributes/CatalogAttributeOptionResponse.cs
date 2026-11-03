namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed record CatalogAttributeOptionResponse(string Code, string DefaultName, string? MediaPath, string? ColorHex);