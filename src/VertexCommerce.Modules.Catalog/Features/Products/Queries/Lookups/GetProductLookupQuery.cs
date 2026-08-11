using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.Lookups;

public record GetProductLookupQuery(string? SearchTerm, int Limit) : IQuery<IReadOnlyList<ProductLookupItem>>;
