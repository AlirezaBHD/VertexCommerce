using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class ProductListResponseType
{
    public IEnumerable<ProductReadModel> Products { get; init; } = [];
    public long TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalPages { get; init; }

    public static ProductListResponseType Empty(int page, int pageSize) => new()
    {
        Products = [],
        TotalCount = 0,
        Page = page,
        PageSize = pageSize,
        TotalPages = 0
    };
}
