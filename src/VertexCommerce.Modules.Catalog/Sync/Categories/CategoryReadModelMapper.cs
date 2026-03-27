using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;

internal static class CategoryReadModelMapper
{
    public static CategoryReadModel ToReadModel(
        Category category,
        List<Category> allCategories,
        int productCount = 0)
    {
        var (path, pathIds, depth) = BuildPath(category, allCategories);

        var childCount = allCategories.Count(c => c.ParentId == category.Id);

        return new CategoryReadModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder,
            Path = path,
            PathIds = pathIds,
            Depth = depth,
            ChildCount = childCount,
            ProductCount = productCount,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    private static (string Path, List<Guid> PathIds, int Depth) BuildPath(
        Category category,
        List<Category> allCategories)
    {
        var pathNames = new List<string>();
        var pathIds = new List<Guid>();

        var current = category;
        while (current is not null)
        {
            pathNames.Add(current.Name);
            pathIds.Add(current.Id);

            current = current.ParentId.HasValue
                ? allCategories.FirstOrDefault(c => c.Id == current.ParentId.Value)
                : null;
        }

        pathNames.Reverse();
        pathIds.Reverse();

        return (
            Path: string.Join(" > ", pathNames),
            PathIds: pathIds,
            Depth: pathIds.Count - 1
        );
    }
}
