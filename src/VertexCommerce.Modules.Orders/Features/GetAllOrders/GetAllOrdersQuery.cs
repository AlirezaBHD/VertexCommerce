using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;

public sealed record GetAllOrdersQuery(Guid? CustomerId) : PagedQuery, IQuery<PagedResult<AllOrdersResponse>>;

