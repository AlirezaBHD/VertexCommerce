namespace VertexCommerce.Modules.Catalog.Features.UpdateCategory;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder
);
