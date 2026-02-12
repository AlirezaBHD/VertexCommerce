using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, CheckoutResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketService _basketService;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public CheckoutCommandHandler(
        IOrderRepository orderRepository,
        IBasketService basketService,
        IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _basketService = basketService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CheckoutResponse>> Handle(CheckoutCommand command, CancellationToken ct)
    {
        var basket = await _basketService.GetBasketAsync(command.CustomerId, ct);

        if (basket is null || basket.Items.Count == 0)
        {
            return Result.Failure<CheckoutResponse>(
                Error.Validation("Basket is empty. Cannot checkout."));
        }

        var shippingAddress = Address.Create(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.Country,
            command.ShippingAddress.ZipCode
        );

        var billingAddress = Address.Create(
                command.BillingAddress.Street,
                command.BillingAddress.City,
                command.BillingAddress.State,
                command.BillingAddress.Country,
                command.BillingAddress.ZipCode
            );

        var order = Order.Create(
            command.CustomerId,
            command.CustomerEmail,
            shippingAddress,
            billingAddress,
            basket.Currency,
            command.Notes
        );

        foreach (var item in basket.Items)
        {
            var unitPrice = Money.Create(item.UnitPrice, basket.Currency);
            
            order.AddItem(
                item.ProductId,
                item.ProductName,
                item.ProductSku,
                unitPrice,
                item.Quantity
            );
        }

        order.Confirm();

        await _orderRepository.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _basketService.ClearBasketAsync(command.CustomerId, ct);

        return Result.Success(new CheckoutResponse(
            order.Id,
            order.OrderNumber,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency
        ));
    }
}