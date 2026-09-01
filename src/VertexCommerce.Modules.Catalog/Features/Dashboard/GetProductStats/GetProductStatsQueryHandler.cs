using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Dashboard.GetProductStats;

internal sealed class GetProductStatsQueryHandler(
    CatalogDbContext dbContext)
    : IQueryHandler<GetProductStatsQuery, ProductStatsResponse>
{
    public async Task<Result<ProductStatsResponse>> Handle(GetProductStatsQuery query, CancellationToken ct)
    {
        var activeCount = await dbContext.Products
            .CountAsync(p => p.IsActive && !p.IsDeleted, ct);

        return Result.Success(new ProductStatsResponse(ActiveProductsCount: activeCount));
    }
}
