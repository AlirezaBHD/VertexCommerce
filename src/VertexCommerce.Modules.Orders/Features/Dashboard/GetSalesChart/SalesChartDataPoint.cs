namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetSalesChart;

public sealed record SalesChartDataPoint(
    DateTime Date,
    decimal TotalSales,
    int OrderCount);
