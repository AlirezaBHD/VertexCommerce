namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder
);
