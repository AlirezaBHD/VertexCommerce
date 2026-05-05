namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed record CatalogAttributesResponse(string Code, string DefaultName, List<CatalogAttributeOptionResponse> Options);

public sealed record CatalogAttributeOptionResponse(string Code, string DefaultName,string? MediaPath );