using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;

namespace VertexCommerce.Modules.Catalog.Sync;

internal sealed class CategoryPathBuilder
{
    private readonly CatalogDbContext _dbContext;

    public CategoryPathBuilder(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CategoryBreadcrumb>> BuildAsync(Guid categoryId, CancellationToken ct)
    {
        var breadcrumb = new List<CategoryBreadcrumb>();
        var currentId = (Guid?)categoryId;
        var maxDepth = 10;

        while (currentId.HasValue && maxDepth-- > 0)
        {
            var category = await _dbContext.Categories
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, c.ParentId, Slug = c.Seo.Slug })
                .FirstOrDefaultAsync(c => c.Id == currentId.Value, ct);

            if (category is null) break;

            breadcrumb.Insert(0, new CategoryBreadcrumb
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            });
            
            currentId = category.ParentId;
        }

        return breadcrumb;
    }
}
