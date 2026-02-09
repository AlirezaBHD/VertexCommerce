namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class ProductListResponseType
{
    public IEnumerable<ProductSummaryType> Products { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }

    public static ProductListResponseType Empty(int page, int pageSize) => new()
    {
        Products = [],
        TotalCount = 0,
        Page = page,
        PageSize = pageSize,
        TotalPages = 0
    };
}
