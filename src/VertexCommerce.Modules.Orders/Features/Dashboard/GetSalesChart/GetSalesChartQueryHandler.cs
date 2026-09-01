using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetSalesChart;

internal sealed class GetSalesChartQueryHandler(
    OrdersDbContext dbContext)
    : IQueryHandler<GetSalesChartQuery, IReadOnlyList<SalesChartDataPoint>>
{
    public async Task<Result<IReadOnlyList<SalesChartDataPoint>>> Handle(
        GetSalesChartQuery query, CancellationToken ct)
    {
        var fromUtc = DateTime.SpecifyKind(query.From.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(query.To.Date.AddDays(1), DateTimeKind.Utc);

        var orders = await dbContext.Orders
            .Where(o => o.CreatedAt >= fromUtc && o.CreatedAt < toUtc)
            .Where(o => o.PaymentStatus == PaymentStatus.Paid)
            .Select(o => new
            {
                o.CreatedAt,
                Amount = o.TotalAmount.Amount
            })
            .ToListAsync(ct);

        var dataPoints = orders
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new SalesChartDataPoint(
                Date: g.Key,
                TotalSales: g.Sum(o => o.Amount),
                OrderCount: g.Count()))
            .OrderBy(dp => dp.Date)
            .ToList();

        return Result.Success<IReadOnlyList<SalesChartDataPoint>>(dataPoints);
    }
}
