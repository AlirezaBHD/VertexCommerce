using VertexCommerce.Modules.Basket.Domain.Entities;
using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed class AddItemCommandHandler : ICommandHandler<AddItemCommand>
{
    private readonly IBasketRepository _basketRepository;

    public AddItemCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(AddItemCommand command, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);

        if (basket is null)
        {
            basket = CustomerBasket.Create(command.CustomerId, command.Currency);
            
            basket.AddItem(
                command.ProductId,
                command.ProductName,
                command.ProductSku,
                command.ImageUrl,
                command.UnitPrice,
                command.Quantity
            );

            await _basketRepository.CreateAsync(basket, ct);
        }
        else
        {
            basket.AddItem(
                command.ProductId,
                command.ProductName,
                command.ProductSku,
                command.ImageUrl,
                command.UnitPrice,
                command.Quantity
            );

            await _basketRepository.UpdateAsync(basket, ct);
        }

        return Result.Success();
    }
}
