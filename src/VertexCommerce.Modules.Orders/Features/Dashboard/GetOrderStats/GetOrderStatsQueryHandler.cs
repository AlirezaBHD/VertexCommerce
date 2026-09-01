using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetOrderStats;

internal sealed class GetOrderStatsQueryHandler(
    OrdersDbContext dbContext)
    : IQueryHandler<GetOrderStatsQuery, OrderStatsResponse>
{
    public async Task<Result<OrderStatsResponse>> Handle(GetOrderStatsQuery query, CancellationToken ct)
    {
        var todayUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);

        var totalSales = await dbContext.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.Paid)
            .SumAsync(o => o.TotalAmount.Amount, ct);

        var todayOrdersCount = await dbContext.Orders
            .CountAsync(o => o.CreatedAt >= todayUtc, ct);

        return Result.Success(new OrderStatsResponse(
            TotalSales: totalSales,
            TodayOrdersCount: todayOrdersCount));
    }
}
