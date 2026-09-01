using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetRecentOrders;

internal sealed class GetRecentOrdersQueryHandler(
    OrdersDbContext dbContext)
    : IQueryHandler<GetRecentOrdersQuery, IReadOnlyList<RecentOrderResponse>>
{
    public async Task<Result<IReadOnlyList<RecentOrderResponse>>> Handle(
        GetRecentOrdersQuery query, CancellationToken ct)
    {
        var orders = await dbContext.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(query.Count)
            .Select(o => new RecentOrderResponse(
                o.Id,
                o.OrderNumber,
                o.CustomerPhoneNumber,
                o.Status.ToString(),
                o.TotalAmount.Amount.ToString("F2"),
                o.CreatedAt))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<RecentOrderResponse>>(orders);
    }
}
