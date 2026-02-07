using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.RemoveItem;

public sealed class RemoveItemCommandHandler : ICommandHandler<RemoveItemCommand>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(RemoveItemCommand command, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);

        if (basket is null)
        {
            return Result.Failure(Error.NotFound("Basket", command.CustomerId));
        }

        basket.RemoveItem(command.ProductId);

        await _basketRepository.UpdateAsync(basket, ct);

        return Result.Success();
    }
}
