using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentId,
    int SortOrder = 0
) : ICommand<Guid>;
