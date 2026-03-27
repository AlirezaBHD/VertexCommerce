using MongoDB.Bson;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

internal static class ProductQueryFilterBuilder
{
    public static FilterDefinition<ProductReadModel> BuildCategoryFilter(
        Guid categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock)
    {
        var builder = Builders<ProductReadModel>.Filter;
        var filter = builder.Eq(p => p.CategoryId, categoryId)
                     & builder.Eq(p => p.IsActive, true);

        if (minPrice.HasValue)
            filter &= builder.Gte(p => p.MinPrice, minPrice.Value);

        if (maxPrice.HasValue)
            filter &= builder.Lte(p => p.MaxPrice, maxPrice.Value);

        if (inStock == true)
            filter &= builder.Gt(p => p.TotalStock, 0);

        return filter;
    }

    public static FilterDefinition<ProductReadModel> BuildSearchFilter(
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null)
    {
        var builder = Builders<ProductReadModel>.Filter;

        var filter = builder.Empty;

        if (isActive.HasValue)
            filter &= builder.Eq(p => p.IsActive, isActive.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            var textFilter = builder.Text(term);
            var regexPattern = new BsonRegularExpression(
                System.Text.RegularExpressions.Regex.Escape(term), "i");
            var fallbackFilter = builder.Regex(p => p.Name, regexPattern);

            filter &= (textFilter | fallbackFilter);
        }

        if (categoryId.HasValue)
            filter &= builder.Eq(p => p.CategoryId, categoryId.Value);

        if (minPrice.HasValue)
            filter &= builder.Gte(p => p.MinPrice, minPrice.Value);

        if (maxPrice.HasValue)
            filter &= builder.Lte(p => p.MaxPrice, maxPrice.Value);

        return filter;
    }


    public static SortDefinition<ProductReadModel> BuildSort(
        string? sortBy,
        bool descending)
    {
        var builder = Builders<ProductReadModel>.Sort;

        return sortBy?.ToLower() switch
        {
            "price" => descending
                ? builder.Descending(p => p.MaxPrice)
                : builder.Ascending(p => p.MinPrice),
            "name" => descending
                ? builder.Descending(p => p.Name)
                : builder.Ascending(p => p.Name),
            "newest" => builder.Descending(p => p.CreatedAt),
            "oldest" => builder.Ascending(p => p.CreatedAt),
            _ => builder.Descending(p => p.CreatedAt)
        };
    }
}
