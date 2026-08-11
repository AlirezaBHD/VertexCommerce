using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;

namespace VertexCommerce.Modules.Catalog.Sync.Categories;

internal sealed class CategorySyncService(
    CatalogDbContext dbContext,
    ICategoryReadModelRepository repository)
    : ICategorySyncService
{
    public async Task SyncCategoryAsync(
        Guid categoryId, CancellationToken ct = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId, ct);

        if (category is null)
        {
            await repository.DeleteAsync(categoryId, ct);
            return;
        }

        var allCategories = await dbContext.Categories
            .AsNoTracking()
            .ToListAsync(ct);

        var productCount = await dbContext.Products
            .AsNoTracking()
            .CountAsync(p => p.CategoryId == categoryId, ct);

        var readModel = CategoryReadModelMapper.ToReadModel(
            category, allCategories, productCount);

        await repository.UpsertAsync(readModel, ct);

        await SyncChildrenAsync(categoryId, allCategories, ct);
    }

    public async Task SyncAllCategoriesAsync(CancellationToken ct = default)
    {
        var allCategories = await dbContext.Categories
            .AsNoTracking()
            .ToListAsync(ct);

        foreach (var category in allCategories)
        {
            var productCount = await dbContext.Products
                .AsNoTracking()
                .CountAsync(p => p.CategoryId == category.Id, ct);

            var readModel = CategoryReadModelMapper.ToReadModel(
                category, allCategories, productCount);

            await repository.UpsertAsync(readModel, ct);
        }
    }

    public async Task DeleteCategoryAsync(
        Guid categoryId, CancellationToken ct = default)
    {
        await repository.DeleteAsync(categoryId, ct);
    }

    private async Task SyncChildrenAsync(
        Guid parentId, List<Domain.Categories.Category> allCategories,
        CancellationToken ct)
    {
        var children = allCategories
            .Where(c => c.ParentId == parentId)
            .ToList();

        foreach (var child in children)
        {
            var productCount = await dbContext.Products
                .AsNoTracking()
                .CountAsync(p => p.CategoryId == child.Id, ct);

            var readModel = CategoryReadModelMapper.ToReadModel(
                child, allCategories, productCount);

            await repository.UpsertAsync(readModel, ct);

            await SyncChildrenAsync(child.Id, allCategories, ct);
        }
    }
}
