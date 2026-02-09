using System.Linq.Expressions;
using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Features.Specifications;

public sealed record ProductSummaryResult(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive,
    string? ImageUrl,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt
);

public sealed class FeaturedProductsSpec : BaseSpecification<Product, ProductSummaryResult>
{
    public FeaturedProductsSpec(int take = 8)
    {
        Where(p => p.IsActive && p.StockQuantity > 0);
        OrderByDesc(p => p.CreatedAt);
        ApplyPaging(0, take);

        Select(p => new ProductSummaryResult(
            p.Id,
            p.Name,
            p.Sku.Value,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.IsActive,
            null,
            p.CategoryId,
            p.Category != null ? p.Category.Name : null,
            p.CreatedAt
        ));
    }
}

public sealed class NewProductsSpec : BaseSpecification<Product, ProductSummaryResult>
{
    public NewProductsSpec(int take = 8)
    {
        Where(p => p.IsActive);
        OrderByDesc(p => p.CreatedAt);
        ApplyPaging(0, take);

        Select(p => new ProductSummaryResult(
            p.Id,
            p.Name,
            p.Sku.Value,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.IsActive,
            null,
            p.CategoryId,
            p.Category != null ? p.Category.Name : null,
            p.CreatedAt
        ));
    }
}

public sealed class ProductsByCategorySpec : BaseSpecification<Product, ProductSummaryResult>
{
    public ProductsByCategorySpec(
        Guid categoryId,
        int skip = 0,
        int take = 20,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        string? sortBy = null,
        bool descending = false)
    {
        Where(p => p.CategoryId == categoryId && p.IsActive);

        if (minPrice.HasValue)
            Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            Where(p => p.Price.Amount <= maxPrice.Value);

        if (inStock == true)
            Where(p => p.StockQuantity > 0);

        ApplySorting(sortBy, descending);
        ApplyPaging(skip, take);

        Select(p => new ProductSummaryResult(
            p.Id,
            p.Name,
            p.Sku.Value,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.IsActive,
            null,
            p.CategoryId,
            p.Category != null ? p.Category.Name : null,
            p.CreatedAt
        ));
    }

    private void ApplySorting(string? sortBy, bool descending)
    {
        Expression<Func<Product, object>> orderExpr = sortBy?.ToLower() switch
        {
            "price" => p => p.Price.Amount,
            "name" => p => p.Name,
            "stock" => p => p.StockQuantity,
            _ => p => p.CreatedAt
        };

        if (descending)
            OrderByDesc(orderExpr);
        else
            OrderByAsc(orderExpr);
    }
}

public sealed class SearchProductsSpec : BaseSpecification<Product, ProductSummaryResult>
{
    public SearchProductsSpec(
        string searchTerm,
        int skip = 0,
        int take = 20,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var term = searchTerm.ToLower();

        Where(p => p.IsActive && (
            p.Name.ToLower().Contains(term) ||
            (p.Description != null && p.Description.ToLower().Contains(term)) ||
            p.Sku.Value.ToLower().Contains(term)
        ));

        if (categoryId.HasValue)
            Where(p => p.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            Where(p => p.Price.Amount <= maxPrice.Value);

        OrderByDesc(p => p.CreatedAt);
        ApplyPaging(skip, take);

        Select(p => new ProductSummaryResult(
            p.Id,
            p.Name,
            p.Sku.Value,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.IsActive,
            null,
            p.CategoryId,
            p.Category != null ? p.Category.Name : null,
            p.CreatedAt
        ));
    }
}

public sealed class ProductCountSpec : BaseSpecification<Product>
{
    public ProductCountSpec(Guid? categoryId = null, string? searchTerm = null)
    {
        Where(p => p.IsActive);

        if (categoryId.HasValue)
            Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)) ||
                p.Sku.Value.ToLower().Contains(term)
            );
        }
    }
}