using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.ClearBasket;

public sealed class ClearBasketCommandHandler : ICommandHandler<ClearBasketCommand>
{
    private readonly IBasketRepository _basketRepository;

    public ClearBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(ClearBasketCommand command, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);

        if (basket is null)
        {
            return Result.Success(); // Already empty
        }

        basket.Clear();

        await _basketRepository.UpdateAsync(basket, ct);

        return Result.Success();
    }
}
