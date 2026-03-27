namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder
);