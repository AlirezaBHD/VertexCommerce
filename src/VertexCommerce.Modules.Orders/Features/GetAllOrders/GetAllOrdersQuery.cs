using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;

public sealed record GetAllOrdersQuery() : PagedQuery, IQuery<PagedResult<AllOrdersResponse>>;

