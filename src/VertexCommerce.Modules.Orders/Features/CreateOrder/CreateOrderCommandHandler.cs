using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders.Features.CreateOrder;

public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var shippingAddress = Address.Create(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.Country,
            command.ShippingAddress.ZipCode
        );

        Address? billingAddress = null;
        if (command.BillingAddress is not null)
        {
            billingAddress = Address.Create(
                command.BillingAddress.Street,
                command.BillingAddress.City,
                command.BillingAddress.State,
                command.BillingAddress.Country,
                command.BillingAddress.ZipCode
            );
        }

        var order = Order.Create(
            command.CustomerId,
            command.CustomerEmail,
            shippingAddress,
            billingAddress,
            command.Currency,
            command.Notes
        );

        foreach (var item in command.Items)
        {
            var unitPrice = Money.Create(item.UnitPrice, command.Currency);
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

        return Result.Success(order.Id);
    }
}