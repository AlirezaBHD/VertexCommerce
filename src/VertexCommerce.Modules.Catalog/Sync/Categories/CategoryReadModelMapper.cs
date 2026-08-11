using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Sync.Categories;

internal static class CategoryReadModelMapper
{
    public static CategoryReadModel ToReadModel(
        Category category,
        List<Category> allCategories,
        int productCount = 0)
    {
        var categoriesById = allCategories.ToDictionary(c => c.Id);

        var (breadcrumb, depth) = BuildPath(category, categoriesById);
        var childCount = allCategories.Count(c => c.ParentId == category.Id);

        return new CategoryReadModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            IconPath = category.IconPath,
            CoverImagePath = category.CoverImagePath,
            ImageAltText = category.ImageAltText,
            IsActive = category.IsActive,
            ShowOnHome = category.ShowOnHome,
            IncludeInMenu = category.IncludeInMenu,
            SortOrder = category.SortOrder,
            Slug = category.Seo.Slug,
            MetaTitle = category.Seo.MetaTitle,
            MetaDescription = category.Seo.MetaDescription,
            Keywords = category.Seo.Keywords,
            Breadcrumb = breadcrumb,
            Depth = depth,
            ChildCount = childCount,
            ProductCount = productCount,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }

    private static (List<CategoryBreadcrumb> Breadcrumb, int Depth) BuildPath(
        Category category,
        IReadOnlyDictionary<Guid, Category> categoriesById)
    {
        var breadcrumb = new List<CategoryBreadcrumb>();

        var current = category;
        while (current is not null)
        {
            breadcrumb.Add(new CategoryBreadcrumb
            {
                Id = current.Id,
                Name = current.Name,
                Slug = current.Seo.Slug
            });

            if (current.ParentId.HasValue &&
                categoriesById.TryGetValue(current.ParentId.Value, out var parent))
            {
                current = parent;
            }
            else
            {
                current = null;
            }
        }

        breadcrumb.Reverse();

        return (
            Breadcrumb: breadcrumb,
            Depth: breadcrumb.Count - 1
        );
    }
}
