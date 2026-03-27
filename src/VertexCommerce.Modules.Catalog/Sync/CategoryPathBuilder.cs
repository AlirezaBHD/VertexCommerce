using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;

namespace VertexCommerce.Modules.Catalog.Sync;

internal sealed class CategoryPathBuilder
{
    private readonly CatalogDbContext _dbContext;

    public CategoryPathBuilder(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> BuildAsync(Guid categoryId, CancellationToken ct)
    {
        var path = new List<string>();
        var currentId = (Guid?)categoryId;
        var maxDepth = 10;

        while (currentId.HasValue && maxDepth-- > 0)
        {
            var category = await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == currentId.Value, ct);

            if (category is null) break;

            path.Insert(0, category.Name);
            currentId = category.ParentId;
        }

        return string.Join(" > ", path);
    }
}
