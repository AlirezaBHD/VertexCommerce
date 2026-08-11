namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.Lookups;

public sealed record ProductLookupItem(
    Guid Id,
    string Title,
    string Slug,
    string? ThumbnailUrl);
