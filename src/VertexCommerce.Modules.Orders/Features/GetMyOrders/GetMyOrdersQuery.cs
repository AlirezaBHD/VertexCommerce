using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrders;

public sealed record GetMyOrdersQuery() : PagedQuery, IQuery<PagedResult<MyOrdersResponse>>;
