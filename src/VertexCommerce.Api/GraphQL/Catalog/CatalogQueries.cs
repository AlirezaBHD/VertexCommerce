using VertexCommerce.Api.GraphQL.Catalog.Types;
using VertexCommerce.Modules.Catalog.ReadModels;

namespace VertexCommerce.Api.GraphQL.Catalog;

public sealed partial class Query
{
    public async Task<IEnumerable<ProductSummaryType>> GetFeaturedProducts(
        [Service] IProductReadModelRepository repository,
        int take = 8,
        CancellationToken ct = default)
    {
        var products = await repository.GetFeaturedAsync(take, ct);
        return products.Select(MapToSummary);
    }

    public async Task<IEnumerable<ProductSummaryType>> GetNewProducts(
        [Service] IProductReadModelRepository repository,
        int take = 8,
        CancellationToken ct = default)
    {
        var products = await repository.GetNewAsync(take, ct);
        return products.Select(MapToSummary);
    }

    public async Task<ProductListResponseType> GetProductsByCategory(
        [Service] IProductReadModelRepository repository,
        Guid categoryId,
        int page = 1,
        int pageSize = 20,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        string? sortBy = null,
        bool descending = false,
        CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;

        var products = await repository.GetByCategoryAsync(
            categoryId, skip, pageSize,
            minPrice, maxPrice, inStock,
            sortBy, descending, ct);

        var totalCount = await repository.CountByCategoryAsync(categoryId, ct);

        return new ProductListResponseType
        {
            Products = products.Select(MapToSummary),
            TotalCount = (int)totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ProductListResponseType> SearchProducts(
        [Service] IProductReadModelRepository repository,
        string searchTerm,
        int page = 1,
        int pageSize = 20,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return ProductListResponseType.Empty(page, pageSize);

        var skip = (page - 1) * pageSize;

        var products = await repository.SearchAsync(
            searchTerm, skip, pageSize,
            categoryId, minPrice, maxPrice, ct);

        var totalCount = await repository.CountSearchAsync(searchTerm, categoryId, ct);

        return new ProductListResponseType
        {
            Products = products.Select(MapToSummary),
            TotalCount = (int)totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ProductDetailType?> GetProductById(
        [Service] IProductReadModelRepository repository,
        Guid id,
        CancellationToken ct = default)
    {
        var product = await repository.GetByIdAsync(id, ct);
        return product is null ? null : MapToDetail(product);
    }

    public async Task<ProductDetailType?> GetProductBySku(
        [Service] IProductReadModelRepository repository,
        string sku,
        CancellationToken ct = default)
    {
        var product = await repository.GetBySkuAsync(sku, ct);
        return product is null ? null : MapToDetail(product);
    }

    #region Mappers

    private static ProductSummaryType MapToSummary(ProductReadModel p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Sku = p.Sku,
        Price = p.Price,
        Currency = p.Currency,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        ImageUrl = null,
        CategoryId = p.CategoryId,
        CategoryName = p.CategoryName,
        CreatedAt = p.CreatedAt
    };

    private static ProductDetailType MapToDetail(ProductReadModel p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Sku = p.Sku,
        Price = p.Price,
        Currency = p.Currency,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        CategoryId = p.CategoryId,
        CategoryName = p.CategoryName,
        CategoryPath = p.CategoryPath,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    #endregion
}