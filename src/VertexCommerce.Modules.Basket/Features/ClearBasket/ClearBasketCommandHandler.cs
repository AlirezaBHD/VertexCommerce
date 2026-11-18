using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.ClearBasket;

internal sealed class ClearBasketCommandHandler(
    IBasketRepository basketRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver) : ICommandHandler<ClearBasketCommand>
{
    public async Task<Result> Handle(ClearBasketCommand command, CancellationToken ct)
    {
        var userId = currentUser.GetUserId();
        var customerId = await customerResolver.ResolveCustomerIdAsync(userId, ct);

        if (customerId is null)
            return Result.Failure(BasketErrors.CustomerNotFound(userId));
        
        await basketRepository.DeleteAsync(customerId.Value, ct);

        return Result.Success();
    }
}
