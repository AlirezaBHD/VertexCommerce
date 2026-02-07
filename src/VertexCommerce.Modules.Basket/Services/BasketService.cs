using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Modules.Basket.Services;

public sealed class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;

    public BasketService(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<BasketDto?> GetBasketAsync(Guid customerId, CancellationToken ct = default)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is null || basket.IsEmpty)
        {
            return null;
        }

        return new BasketDto(
            basket.CustomerId,
            basket.Currency,
            basket.Items.Select(i => new BasketItemDto(
                i.ProductId,
                i.ProductName,
                i.ProductSku,
                i.UnitPrice,
                i.Quantity
            )).ToList()
        );
    }

    public async Task ClearBasketAsync(Guid customerId, CancellationToken ct = default)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is not null)
        {
            basket.Clear();
            await _basketRepository.UpdateAsync(basket, ct);
        }
    }
}
