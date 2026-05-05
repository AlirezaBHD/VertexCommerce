using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed record GetCatalogAttributesQuery() : IQuery<IReadOnlyList<CatalogAttributesResponse>>;
