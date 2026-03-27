using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder
) : ICommand;
