using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Contracts.Baskets;

namespace VertexCommerce.Modules.Basket.Services;

public sealed class BasketService(IBasketRepository basketRepository) : IBasketService
{
    public async Task<BasketDto?> GetBasketAsync(Guid customerId,
        CancellationToken ct = default)
    {
        var basket = await basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is null || basket.Items.Count < 1)
        {
            return null;
        }

        return new BasketDto(
            Id: basket.Id,
            CustomerId: basket.CustomerId,
            Items: basket.Items.Select(i => new BasketItemDto(
                ProductId: i.ProductId,
                VariantId: i.VariantId,
                Sku: i.Sku,
                ProductName: i.ProductName,
                UnitPrice: i.Price,
                Quantity: i.Quantity,
                Attributes: i.Attributes.Select(a => new BasketItemAttributeDto(
                    AttributeCode: a.AttributeCode, OptionCode: a.OptionCode)).ToList()
            )).ToList()
        );
    }

    public async Task ClearBasketAsync(Guid customerId, CancellationToken ct = default)
    {
        await basketRepository.DeleteAsync(customerId: customerId, ct);
    }
}
