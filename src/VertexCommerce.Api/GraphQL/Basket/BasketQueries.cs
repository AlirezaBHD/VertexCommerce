using VertexCommerce.Api.GraphQL.Basket.Types;
using VertexCommerce.Modules.Basket.Domain.Repositories;

namespace VertexCommerce.Api.GraphQL.Basket;

public sealed partial class Query
{
    public async Task<BasketType?> GetBasket(
        [Service] IBasketRepository basketRepository,
        Guid customerId,
        CancellationToken ct = default)
    {
        var basket = await basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is null)
            return null;

        return new BasketType
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            Currency = basket.Currency,
            TotalAmount = basket.TotalAmount,
            TotalItems = basket.TotalItems,
            CreatedAt = basket.CreatedAt,
            ExpiresAt = basket.ExpiresAt,
            Items = basket.Items.Select(i => new BasketItemType
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductSku = i.ProductSku,
                ImageUrl = i.ImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice,
                AddedAt = i.AddedAt
            }).ToList()
        };
    }
}
