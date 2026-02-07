using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(query.Id, ct);

        if (order is null)
        {
            return Result.Failure<OrderResponse>(Error.NotFound("Order", query.Id));
        }

        var response = new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.CustomerEmail,
            order.Status,
            order.PaymentStatus,
            new AddressResponse(
                order.ShippingAddress.Street,
                order.ShippingAddress.City,
                order.ShippingAddress.State,
                order.ShippingAddress.Country,
                order.ShippingAddress.ZipCode
            ),
            order.BillingAddress is not null
                ? new AddressResponse(
                    order.BillingAddress.Street,
                    order.BillingAddress.City,
                    order.BillingAddress.State,
                    order.BillingAddress.Country,
                    order.BillingAddress.ZipCode
                )
                : null,
            order.SubTotal.Amount,
            order.ShippingCost.Amount,
            order.Tax.Amount,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.Notes,
            order.CreatedAt,
            order.ShippedAt,
            order.DeliveredAt,
            order.CancelledAt,
            order.CancellationReason,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.ProductSku,
                i.UnitPrice.Amount,
                i.Quantity,
                i.TotalPrice.Amount
            )).ToList()
        );

        return Result.Success(response);
    }
}