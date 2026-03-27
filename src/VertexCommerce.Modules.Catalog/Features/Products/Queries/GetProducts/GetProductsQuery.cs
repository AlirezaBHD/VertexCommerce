using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? SearchTerm = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20
) : IQuery<PagedResult<ProductListItem>>;
