using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ICustomerService customerService)
    : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
{
    public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var spec = new GetOrderByIdSpec(query.OrderId);

        var orderResponse = await orderRepository.GetOrderByIdAsync(spec, ct);

        if (orderResponse == null)
        {
            return Result.Failure<GetOrderByIdResponse>(Error.NotFound("Order", query.OrderId));
        }
        
        var customerInfo = await customerService.GetCustomerInfo(orderResponse.CustomerId, ct);
        var customerName = customerInfo != null 
            ? $"{customerInfo.FirstName} {customerInfo.LastName}".Trim() 
            : null;
            
        var finalResponse = orderResponse with { CustomerName = customerName };
        
        return Result.Success(finalResponse);
    }
}
