using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed class GetMyOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : IQueryHandler<GetMyOrderByIdQuery, MyOrderResponse>
{
    public async Task<Result<MyOrderResponse>> Handle(GetMyOrderByIdQuery query, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var spec = new GetMyOrderByIdSpec(customerId: customerId, orderId: query.OrderId);
        var order = await orderRepository.GetOrderByIdAsync(spec, ct);

        if (order is null)
        {
            return Result.Failure<MyOrderResponse>(Error.NotFound("Order", query.OrderId));
        }


        return Result.Success(order);
    }
}
