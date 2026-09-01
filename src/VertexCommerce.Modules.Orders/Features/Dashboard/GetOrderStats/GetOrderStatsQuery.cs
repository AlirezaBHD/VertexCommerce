using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetOrderStats;

public sealed record GetOrderStatsQuery : IQuery<OrderStatsResponse>;
