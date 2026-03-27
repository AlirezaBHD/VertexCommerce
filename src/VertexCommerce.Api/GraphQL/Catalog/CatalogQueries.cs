using VertexCommerce.Api.GraphQL.Catalog.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog;

[ExtendObjectType(typeof(Query))]
public sealed class CatalogQueries
{
    [UseProjection]
    [UseSorting]
    public IExecutable<ProductReadModel> GetProducts(
        [Service] IProductReadModelRepository repository)
    {
        return repository.GetProducts();
    }
    
    [UseProjection]
    [UseFirstOrDefault]
    public IExecutable<ProductReadModel> GetProductById(
        Guid id,
        [Service] IProductReadModelRepository repository)
    {
        return repository.GetByIdAsync(id);
    }
    [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 50)]
    [UseProjection]
    [UseSorting]
    public IExecutable<ProductReadModel> GetPaginatedProducts(
        [Service] IProductReadModelRepository repository,
        string? searchTerm,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isActive)
    {
        return repository.GetFilteredProducts(
            searchTerm, categoryId, minPrice, maxPrice, isActive);
    }

    [UseProjection]
    [UseSorting]
    public IExecutable<ProductReadModel> GetProductsByCategory(
        Guid categoryId,
        bool? inStock,
        decimal? minPrice,
        decimal? maxPrice,
        [Service] IProductReadModelRepository repository)
    {
        return repository.GetProductsByCategory(categoryId, inStock, minPrice, maxPrice);
    }
    
    public async Task<ProductListResponseType> SearchProducts(
        string searchTerm,
        int skip = 0,
        int take = 20,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        [Service] IProductReadModelRepository repository = default!,
        CancellationToken ct = default)
    {
        var (items, totalCount) = await repository.SearchAsync(
            searchTerm, skip, take,
            categoryId, minPrice, maxPrice, ct);

        return new ProductListResponseType
        {
            Products = items,
            TotalCount = totalCount,
            Page = 0,
            PageSize = 20,
            TotalPages = long.Parse((totalCount / take).ToString())
        };
    }
    //----
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IExecutable<CategoryReadModel> GetCategories(
        [Service] ICategoryReadModelRepository repository,
        bool? isActive,
        CancellationToken ct)
    {
        return repository.GetAllAsync(isActive, ct);
    }

    [UseProjection]
    public IExecutable<CategoryReadModel> GetCategoryById(
        [Service] ICategoryReadModelRepository repository,
        Guid id,
        CancellationToken ct)
    {
        return repository.GetByIdAsync(id, ct);
    }

    /// <summary>
    /// Returns only root categories (tree entry points)
    /// </summary>
    public async Task<List<CategoryReadModel>> GetCategoryTree(
        [Service] ICategoryReadModelRepository repository,
        CancellationToken ct)
    {
        return await repository.GetRootCategoriesAsync(ct);
    }
}