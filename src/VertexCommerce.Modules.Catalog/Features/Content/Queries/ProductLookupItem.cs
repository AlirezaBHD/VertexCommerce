namespace VertexCommerce.Modules.Catalog.Features.Content.Queries;

public sealed record ProductLookupItem(
    Guid Id,
    string Title,
    string Slug,
    string? ThumbnailUrl);
