using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.Lookups;

public record GetCategoryLookupQuery(string? SearchTerm, int Limit) : IQuery<IReadOnlyList<CategoryLookupItem>>;