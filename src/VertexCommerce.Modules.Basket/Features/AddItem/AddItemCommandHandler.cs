using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Basket.Configuration;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Persistence.Documents;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

internal sealed class AddItemCommandHandler(
    IBasketRepository basketRepository,
    IProductService productService,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    BasketFactory basketFactory,
    IOptions<BasketSettings> settings) : ICommandHandler<AddItemCommand>
{
    private readonly BasketSettings _settings = settings.Value;

    public async Task<Result> Handle(AddItemCommand command, CancellationToken ct)
    {
        var customerId = await ResolveCustomerIdAsync(ct);

        if (customerId is null)
            return Result.Failure(BasketErrors.CustomerNotFound(currentUser.UserId));

        var variant = await productService.GetProductVariantInfoAsync(
            command.ProductId,
            command.VariantId,
            ct);

        if (variant is null)
            return Result.Failure(BasketErrors.VariantNotFound(command.ProductId, command.VariantId));

        var basket = await basketRepository.GetByCustomerIdAsync(customerId.Value, ct)
                     ?? basketFactory.CreateNew(customerId.Value);

        var result = TryAddOrUpdateItem(basket, variant, command.Quantity);

        if (result.IsFailure)
            return result;

        basketFactory.RefreshExpiration(basket);

        await basketRepository.UpsertAsync(basket, ct);

        return Result.Success();
    }


    private async Task<Guid?> ResolveCustomerIdAsync(CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(
            currentUser.UserId, ct);

        return customerId == Guid.Empty ? null : customerId;
    }

    private Result TryAddOrUpdateItem(
        BasketDocument basket,
        ProductVariantInfo variant,
        int quantity)
    {
        var existingItem = basket.Items.FirstOrDefault(i =>
            i.ProductId == variant.ProductId &&
            i.VariantId == variant.VariantId);

        var totalQuantity = (existingItem?.Quantity ?? 0) + quantity;

        if (totalQuantity <= 0 && existingItem is not null)
        {
            basket.Items.Remove(existingItem);
        }

        if (totalQuantity > _settings.MaxQuantityPerItem)
            return Result.Failure(BasketErrors.MaxQuantityExceeded(_settings.MaxQuantityPerItem));

        if (totalQuantity > variant.StockQuantity)
        {
            return existingItem is not null
                ? Result.Failure(BasketErrors.QuantityExceedsStock(
                    variant.Sku, existingItem.Quantity, quantity, variant.StockQuantity))
                : Result.Failure(BasketErrors.InsufficientStock(
                    variant.Sku, variant.StockQuantity, quantity));
        }

        if (existingItem is not null)
        {
            existingItem.Quantity = totalQuantity;
            existingItem.Price = variant.Price; // refresh snapshot
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            if (basket.Items.Count >= _settings.MaxItemsInBasket)
                return Result.Failure(BasketErrors.BasketIsFull(_settings.MaxItemsInBasket));
            
            basket.Items.Add(BasketItemMapper.ToDocument(variant, quantity));
        }

        basket.TotalItems = basket.Items.Sum(i => i.Quantity);
        basket.TotalAmount = basket.Items.Sum(i => i.Price);
        return Result.Success();
    }
}
