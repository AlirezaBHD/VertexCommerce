using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository)
    : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
{
    public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var spec = new GetOrderByIdSpec(query.OrderId);

        var orders = await orderRepository.GetOrderByIdAsync(spec, ct);

        if (orders == null)
        {
            return Result.Failure<GetOrderByIdResponse>(Error.NotFound("Order", query.OrderId));
        }
        
        return Result.Success(orders);
    }
}
