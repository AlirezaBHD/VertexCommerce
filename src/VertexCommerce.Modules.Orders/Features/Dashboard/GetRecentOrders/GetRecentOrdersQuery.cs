using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetRecentOrders;

public sealed record GetRecentOrdersQuery(int Count = 5) : IQuery<IReadOnlyList<RecentOrderResponse>>;
