using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Specifications;

public sealed record CategoryResult(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentId,
    bool IsActive,
    int SortOrder
);

public sealed class ActiveCategoriesSpec : BaseSpecification<Category, CategoryResult>
{
    public ActiveCategoriesSpec()
    {
        Where(c => c.IsActive);
        OrderByAsc(c => c.SortOrder);

        Select(c => new CategoryResult(
            c.Id,
            c.Name,
            c.Description,
            c.ParentId,
            c.IsActive,
            c.SortOrder
        ));
    }
}

public sealed class RootCategoriesSpec : BaseSpecification<Category, CategoryResult>
{
    public RootCategoriesSpec()
    {
        Where(c => c.IsActive && c.ParentId == null);
        OrderByAsc(c => c.SortOrder);

        Select(c => new CategoryResult(
            c.Id,
            c.Name,
            c.Description,
            c.ParentId,
            c.IsActive,
            c.SortOrder
        ));
    }
}
