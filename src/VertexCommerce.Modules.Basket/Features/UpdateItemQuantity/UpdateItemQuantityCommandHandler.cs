using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Basket.Configuration;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Persistence.Documents;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

internal sealed class UpdateItemQuantityCommandHandler(
    IBasketRepository basketRepository,
    IProductService productService,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    BasketFactory basketFactory,
    IOptions<BasketSettings> settings) : ICommandHandler<UpdateItemQuantityCommand>
{
    private readonly BasketSettings _settings = settings.Value;

    public async Task<Result> Handle(UpdateItemQuantityCommand command, CancellationToken ct)
    {
        var customerId = await ResolveCustomerIdAsync(ct);

        if (customerId is null)
            return Result.Failure(BasketErrors.CustomerNotFound(currentUser.UserId));

        var basket = await basketRepository.GetByCustomerIdAsync(customerId.Value, ct)
                     ?? basketFactory.CreateNew(customerId.Value);

        var existingItem = basket.Items.FirstOrDefault(i =>
            i.ProductId == command.ProductId &&
            i.VariantId == command.VariantId);

        if (existingItem is null)
            return Result.Failure(BasketErrors.VariantNotFound(command.ProductId, command.VariantId));

        if (command.Quantity <= 0)
            return await RemoveItemAsync(basket, existingItem, ct);

        var variant = await productService.GetProductVariantInfoAsync(
            command.ProductId,
            command.VariantId,
            ct);

        if (variant is null)
            return Result.Failure(BasketErrors.VariantNotFound(command.ProductId, command.VariantId));

        var result = TryUpdateItemQuantity(existingItem, variant, command.Quantity);

        if (result.IsFailure)
            return result;

        basket.TotalItems = basket.Items.Sum(i => i.Quantity);
        basket.TotalAmount = basket.Items.Sum(i => i.Price * i.Quantity);

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

    private Result TryUpdateItemQuantity(
        BasketItemDocument item,
        ProductVariantInfo variant,
        int newQuantity)
    {
        if (newQuantity > _settings.MaxQuantityPerItem)
            return Result.Failure(BasketErrors.MaxQuantityExceeded(_settings.MaxQuantityPerItem));

        if (newQuantity > variant.StockQuantity)
            return Result.Failure(BasketErrors.InsufficientStock(
                variant.Sku, variant.StockQuantity, newQuantity));

        item.Quantity = newQuantity;
        item.Price = variant.Price;
        item.TotalPrice = variant.Price * newQuantity;
        item.UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private async Task<Result> RemoveItemAsync(
        BasketDocument basket,
        BasketItemDocument item,
        CancellationToken ct)
    {
        basket.Items.Remove(item);
        basket.TotalItems = basket.Items.Sum(i => i.Quantity);
        basket.TotalAmount = basket.Items.Sum(i => i.Price * i.Quantity);

        basketFactory.RefreshExpiration(basket);

        await basketRepository.UpsertAsync(basket, ct);

        return Result.Success();
    }
}
