namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed record CatalogAttributesResponse(string Code, string DefaultName, List<CatalogAttributeOptionResponse> Options);