using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerStats;

internal sealed class GetCustomerStatsQueryHandler(
    CustomersDbContext dbContext)
    : IQueryHandler<GetCustomerStatsQuery, CustomerStatsResponse>
{
    public async Task<Result<CustomerStatsResponse>> Handle(
        GetCustomerStatsQuery query,
        CancellationToken ct)
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var newCustomersCount = await dbContext.Customers
            .CountAsync(c => c.CreatedAt >= sevenDaysAgo && !c.IsDeleted, ct);

        return Result.Success(new CustomerStatsResponse(newCustomersCount));
    }
}
