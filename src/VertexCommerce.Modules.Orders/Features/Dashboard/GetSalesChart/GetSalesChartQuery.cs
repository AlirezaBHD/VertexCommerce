using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetSalesChart;

public sealed record GetSalesChartQuery(DateTime From, DateTime To) : IQuery<IReadOnlyList<SalesChartDataPoint>>;
