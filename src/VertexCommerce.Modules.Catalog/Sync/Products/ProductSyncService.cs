using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;

namespace VertexCommerce.Modules.Catalog.Sync;

internal sealed class ProductSyncService(
    CatalogDbContext dbContext,
    IProductReadModelRepository readModelRepository,
    CategoryPathBuilder categoryPathBuilder,
    ILogger<ProductSyncService> logger)
    : IProductSyncService
{
    public async Task SyncProductAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await LoadProductWithRelations(productId, ct);

        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found, removing from read store", productId);
            await readModelRepository.DeleteAsync(productId, ct);
            return;
        }

        var categoryPath = await categoryPathBuilder.BuildAsync(product.CategoryId, ct);
        var readModel = ProductReadModelMapper.Map(
            product,
            product.Category?.Name ?? "Unknown",
            categoryPath);

        await readModelRepository.UpsertAsync(readModel, ct);

        logger.LogInformation(
            "Synced product {ProductId} with {VariantCount} variants",
            product.Id, product.Variants.Count);
    }

    public async Task DeleteProductAsync(Guid productId, CancellationToken ct = default)
    {
        await readModelRepository.DeleteAsync(productId, ct);
        logger.LogInformation("Deleted product {ProductId} from read store", productId);
    }

    public async Task SyncAllProductsAsync(CancellationToken ct = default)
    {
        var products = await LoadAllProducts(ct);

        if (products.Count == 0)
        {
            logger.LogWarning("No products found for full sync");
            return;
        }

        var categoryPathCache = new Dictionary<Guid, string>();
        var readModels = new List<ProductReadModel>();

        foreach (var product in products)
        {
            if (!categoryPathCache.TryGetValue(product.CategoryId, out var path))
            {
                path = await categoryPathBuilder.BuildAsync(product.CategoryId, ct);
                categoryPathCache[product.CategoryId] = path;
            }

            readModels.Add(ProductReadModelMapper.Map(
                product,
                product.Category?.Name ?? "Unknown",
                path));
        }

        await readModelRepository.UpsertManyAsync(readModels, ct);

        logger.LogInformation("Full sync completed: {Count} products", readModels.Count);
    }

    public async Task SyncCategoryProductsAsync(Guid categoryId, CancellationToken ct = default)
    {
        var products = await LoadCategoryProducts(categoryId, ct);
        var category = await dbContext.Categories.FindAsync([categoryId], ct);
        var categoryPath = await categoryPathBuilder.BuildAsync(categoryId, ct);
        var categoryName = category?.Name ?? "Unknown";

        var readModels = products
            .Select(p => ProductReadModelMapper.Map(p, categoryName, categoryPath))
            .ToList();

        await readModelRepository.UpsertManyAsync(readModels, ct);

        logger.LogInformation(
            "Category sync: {Count} products in {CategoryId}",
            readModels.Count, categoryId);
    }

    private async Task<Product?> LoadProductWithRelations(Guid productId, CancellationToken ct)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .Include(p => p.Variants).ThenInclude(v => v.Options)
            .Include(p => p.Variants).ThenInclude(v => v.Media)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, ct);
    }

    private async Task<List<Product>> LoadAllProducts(CancellationToken ct)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .Include(p => p.Variants).ThenInclude(v => v.Options)
            .Include(p => p.Variants).ThenInclude(v => v.Media)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    private async Task<List<Product>> LoadCategoryProducts(Guid categoryId, CancellationToken ct)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .Include(p => p.Variants).ThenInclude(v => v.Options)
            .Include(p => p.Variants).ThenInclude(v => v.Media)
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
