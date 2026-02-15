using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Modules.Catalog.ReadModels;

namespace VertexCommerce.Modules.Catalog.Sync;

internal sealed class ProductSyncService : IProductSyncService
{
    private readonly CatalogDbContext _dbContext;
    private readonly IProductReadModelRepository _readModelRepository;

    public ProductSyncService(
        CatalogDbContext dbContext,
        IProductReadModelRepository readModelRepository)
    {
        _dbContext = dbContext;
        _readModelRepository = readModelRepository;
    }

    public async Task SyncProductAsync(Product product, CancellationToken ct = default)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        var categoryPath = category is not null
            ? await BuildCategoryPathAsync(category.Id, ct)
            : null;

        var readModel = MapToReadModel(product, category?.Name ?? "Unknown", categoryPath);
        await _readModelRepository.UpsertAsync(readModel, ct);
    }

    public async Task SyncProductAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
        {
            await _readModelRepository.DeleteAsync(productId, ct);
            return;
        }

        await SyncProductAsync(product, ct);
    }

    public async Task DeleteProductAsync(Guid productId, CancellationToken ct = default)
    {
        await _readModelRepository.DeleteAsync(productId, ct);
    }

    public async Task SyncAllProductsAsync(CancellationToken ct = default)
    {
        var products = await _dbContext.Products
            .Include(p => p.Category)
            .ToListAsync(ct);

        var categoryPaths = new Dictionary<Guid, string>();

        var readModels = new List<ProductReadModel>();

        foreach (var product in products)
        {
            if (!categoryPaths.TryGetValue(product.CategoryId, out var path))
            {
                path = await BuildCategoryPathAsync(product.CategoryId, ct);
                categoryPaths[product.CategoryId] = path;
            }

            readModels.Add(MapToReadModel(
                product,
                product.Category?.Name ?? "Unknown",
                path));
        }

        await _readModelRepository.UpsertManyAsync(readModels, ct);
    }

    public async Task SyncCategoryProductsAsync(Guid categoryId, CancellationToken ct = default)
    {
        var products = await _dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);

        var categoryPath = await BuildCategoryPathAsync(categoryId, ct);
        var category = await _dbContext.Categories.FindAsync([categoryId], ct);

        var readModels = products.Select(p => MapToReadModel(
            p,
            category?.Name ?? "Unknown",
            categoryPath
        )).ToList();

        await _readModelRepository.UpsertManyAsync(readModels, ct);
    }

    private async Task<string> BuildCategoryPathAsync(Guid categoryId, CancellationToken ct)
    {
        var path = new List<string>();
        var currentId = (Guid?)categoryId;

        while (currentId.HasValue)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == currentId.Value, ct);

            if (category is null) break;

            path.Insert(0, category.Name);
            currentId = category.ParentId;
        }

        return string.Join(" > ", path);
    }

    private static ProductReadModel MapToReadModel(Product product, string categoryName, string? categoryPath)
    {
        var searchText = BuildSearchText(product, categoryName);

        return new ProductReadModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku.Value,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            CategoryPath = categoryPath,
            SearchText = searchText,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static string BuildSearchText(Product product, string categoryName)
    {
        var parts = new List<string>
        {
            product.Name,
            product.Sku.Value,
            categoryName
        };

        if (!string.IsNullOrWhiteSpace(product.Description))
        {
            parts.Add(product.Description);
        }

        return string.Join(" ", parts).ToLower();
    }
}