using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.RemoveItem;

internal sealed class RemoveItemCommandHandler(
    IBasketRepository basketRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    BasketFactory basketFactory) : ICommandHandler<RemoveItemCommand>
{
    public async Task<Result> Handle(RemoveItemCommand command, CancellationToken ct)
    {
        var userId = currentUser.GetUserId();
        var customerId = await customerResolver.ResolveCustomerIdAsync(userId, ct);

        if (customerId is null)
            return Result.Failure(BasketErrors.CustomerNotFound(userId));

        var basket = await basketRepository.GetByCustomerIdAsync(customerId.Value, ct);

        if (basket is null)
            return Result.Failure(BasketErrors.BasketNotFound(customerId.Value));

        if (basket.FindItem(command.VariantId) is null)
            return Result.Failure(BasketErrors.ItemNotFound(command.VariantId));

        basket.RemoveItem(command.VariantId);
        basketFactory.RefreshExpiration(basket);

        await basketRepository.UpsertAsync(basket, ct);

        return Result.Success();
    }
}
