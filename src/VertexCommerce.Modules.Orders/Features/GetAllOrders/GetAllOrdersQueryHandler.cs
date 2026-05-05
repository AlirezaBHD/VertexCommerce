using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;

internal sealed class GetAllOrdersQueryHandler(
    IOrderRepository orderRepository)
    : IQueryHandler<GetAllOrdersQuery, PagedResult<AllOrdersResponse>>
{
    public async Task<Result<PagedResult<AllOrdersResponse>>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var spec = new GetAllOrdersSpec();

        var orders = await orderRepository.GetPaginatedAsync
            (spec, skip: query.Skip, take: query.Take, ct);

        return Result.Success(orders);
    }
}
