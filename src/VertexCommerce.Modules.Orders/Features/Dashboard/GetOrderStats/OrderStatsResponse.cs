namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetOrderStats;

public sealed record OrderStatsResponse(
    decimal TotalSales,
    int TodayOrdersCount);
