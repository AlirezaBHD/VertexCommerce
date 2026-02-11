namespace VertexCommerce.Modules.Catalog.Features.CreateCategory;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder
);