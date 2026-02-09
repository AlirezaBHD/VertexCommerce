using VertexCommerce.Api.GraphQL.Catalog.Types;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Features.Specifications;

namespace VertexCommerce.Api.GraphQL.Catalog;

public sealed partial class Query
{
    public async Task<IEnumerable<ProductSummaryType>> GetFeaturedProducts(
        [Service] IProductRepository productRepository,
        int take = 8,
        CancellationToken ct = default)
    {
        var spec = new FeaturedProductsSpec(take);
        var products = await productRepository.ListAsync(spec, ct);
        return products.Select(MapToProductSummary);
    }

    public async Task<IEnumerable<ProductSummaryType>> GetNewProducts(
        [Service] IProductRepository productRepository,
        int take = 8,
        CancellationToken ct = default)
    {
        var spec = new NewProductsSpec(take);
        var products = await productRepository.ListAsync(spec, ct);
        return products.Select(MapToProductSummary);
    }

    public async Task<IEnumerable<CategoryTreeType>> GetCategoryTree(
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct = default)
    {
        var spec = new ActiveCategoriesSpec();
        var allCategories = await categoryRepository.ListAsync(spec, ct);
        return BuildCategoryTree(allCategories.ToList(), null);
    }

    public async Task<ProductListResponseType> GetProductsByCategory(
        [Service] IProductRepository productRepository,
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

        var spec = new ProductsByCategorySpec(
            categoryId, skip, pageSize,
            minPrice, maxPrice, inStock,
            sortBy, descending);

        var countSpec = new ProductCountSpec(categoryId: categoryId);

        var products = await productRepository.ListAsync(spec, ct);
        var totalCount = await productRepository.CountAsync(countSpec, ct);

        return new ProductListResponseType
        {
            Products = products.Select(MapToProductSummary),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ProductListResponseType> SearchProducts(
        [Service] IProductRepository productRepository,
        string searchTerm,
        int page = 1,
        int pageSize = 20,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new ProductListResponseType
            {
                Products = [],
                TotalCount = 0,
                Page = page,
                PageSize = pageSize,
                TotalPages = 0
            };

        var skip = (page - 1) * pageSize;

        var spec = new SearchProductsSpec(
            searchTerm, skip, pageSize,
            categoryId, minPrice, maxPrice);

        var countSpec = new ProductCountSpec(categoryId, searchTerm);

        var products = await productRepository.ListAsync(spec, ct);
        var totalCount = await productRepository.CountAsync(countSpec, ct);

        return new ProductListResponseType
        {
            Products = products.Select(MapToProductSummary),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<IEnumerable<CategoryType>> GetCategories(
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct = default)
    {
        var spec = new ActiveCategoriesSpec();
        var categories = await categoryRepository.ListAsync(spec, ct);
        return categories.Select(MapToCategory);
    }

    public async Task<CategoryType?> GetCategoryById(
        [Service] ICategoryRepository categoryRepository,
        Guid id,
        CancellationToken ct = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, ct);

        if (category is null || !category.IsActive)
            return null;

        return new CategoryType
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder
        };
    }

    public async Task<ProductDetailType?> GetProductById(
        [Service] IProductRepository productRepository,
        Guid id,
        CancellationToken ct = default)
    {
        var product = await productRepository.GetByIdAsync(id, ct);
        return product is null ? null : MapToProductDetail(product);
    }

    public async Task<ProductDetailType?> GetProductBySku(
        [Service] IProductRepository productRepository,
        string sku,
        CancellationToken ct = default)
    {
        var product = await productRepository.GetBySkuAsync(sku, ct);
        return product is null ? null : MapToProductDetail(product);
    }

    #region Mappers

    private static ProductSummaryType MapToProductSummary(ProductSummaryResult p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Sku = p.Sku,
        Price = p.Price,
        Currency = p.Currency,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        ImageUrl = p.ImageUrl,
        CategoryId = p.CategoryId,
        CategoryName = p.CategoryName,
        CreatedAt = p.CreatedAt
    };

    private static ProductDetailType MapToProductDetail(
        Modules.Catalog.Domain.Entities.Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Sku = p.Sku.Value,
        Price = p.Price.Amount,
        Currency = p.Price.Currency,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    private static CategoryType MapToCategory(CategoryResult c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        ParentId = c.ParentId,
        IsActive = c.IsActive,
        SortOrder = c.SortOrder
    };

    private static List<CategoryTreeType> BuildCategoryTree(
        List<CategoryResult> allCategories,
        Guid? parentId)
    {
        return allCategories
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryTreeType
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                Children = BuildCategoryTree(allCategories, c.Id)
            })
            .ToList();
    }

    #endregion
}