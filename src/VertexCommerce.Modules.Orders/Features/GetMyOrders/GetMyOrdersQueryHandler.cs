using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrders;

internal sealed class GetMyOrdersQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : IQueryHandler<GetMyOrdersQuery, PagedResult<MyOrdersResponse>>
{
    public async Task<Result<PagedResult<MyOrdersResponse>>> Handle(GetMyOrdersQuery query, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);
        var spec = new GetMyOrdersSpec(customerId);

        var orders = await orderRepository.GetPaginatedAsync
            (spec, skip: query.Skip, take: query.Take, ct);

        return Result.Success(orders);
    }
}
