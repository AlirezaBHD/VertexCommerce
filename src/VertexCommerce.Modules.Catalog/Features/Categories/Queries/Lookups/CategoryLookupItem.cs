namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.Lookups;

public sealed record CategoryLookupItem(
    Guid Id,
    string Title,
    string Slug,
    List<VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents.CategoryBreadcrumb> Breadcrumb);
