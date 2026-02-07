using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandHandler : ICommandHandler<UpdateItemQuantityCommand>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateItemQuantityCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(UpdateItemQuantityCommand command, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);

        if (basket is null)
        {
            return Result.Failure(Error.NotFound("Basket", command.CustomerId));
        }

        if (!basket.HasItem(command.ProductId))
        {
            return Result.Failure(Error.NotFound("BasketItem", command.ProductId));
        }

        basket.UpdateItemQuantity(command.ProductId, command.Quantity);

        await _basketRepository.UpdateAsync(basket, ct);

        return Result.Success();
    }
}
