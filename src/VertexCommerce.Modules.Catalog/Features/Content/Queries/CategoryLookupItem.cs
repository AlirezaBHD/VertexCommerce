namespace VertexCommerce.Modules.Catalog.Features.Content.Queries;

public sealed record CategoryLookupItem(
    Guid Id,
    string Title,
    string Slug,
    string? Path);
