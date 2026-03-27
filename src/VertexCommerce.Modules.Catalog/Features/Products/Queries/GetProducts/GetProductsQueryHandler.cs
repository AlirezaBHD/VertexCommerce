using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductListItem>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<ProductListItem>>> Handle(GetProductsQuery query, CancellationToken ct)
    {
        //TODO
        // For now, get all and filter in memory
        // Later we'll optimize with proper database queries
        var allProducts = await _productRepository.GetAllAsync(ct);

        var filtered = allProducts.AsEnumerable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchLower = query.SearchTerm.ToLowerInvariant();
            filtered = filtered.Where(p =>
                p.Name.ToLowerInvariant().Contains(searchLower)
                // || p.Sku.Value.ToLowerInvariant().Contains(searchLower)
                );
        }

        if (query.CategoryId.HasValue)
        {
            filtered = filtered.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        // if (query.MinPrice.HasValue)
        // {
        //     filtered = filtered.Where(p => p.Price.Amount >= query.MinPrice.Value);
        // }

        // if (query.MaxPrice.HasValue)
        // {
        //     filtered = filtered.Where(p => p.Price.Amount <= query.MaxPrice.Value);
        // }

        if (query.IsActive.HasValue)
        {
            filtered = filtered.Where(p => p.IsActive == query.IsActive.Value);
        }

        var totalCount = filtered.Count();

        var items = filtered
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductListItem(
                p.Id,
                p.Name,
                p.IsActive,
                p.CategoryId,
                p.Category?.Name,
                p.CreatedAt
            ))
            .ToList();

        var result = new PagedResult<ProductListItem>(items, totalCount, query.Page, query.PageSize);

        return Result.Success(result);
    }
}