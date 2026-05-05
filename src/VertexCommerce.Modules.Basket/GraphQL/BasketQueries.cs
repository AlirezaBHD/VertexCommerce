using HotChocolate;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Persistence.Documents;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Basket.GraphQL;

[ExtendObjectType("Query")]
public sealed class BasketQueries{
    public async Task<BasketDocument?> GetBasket(
        [Service] IBasketRepository basketRepository,
        [Service] ICurrentUser currentUser,
        [Service] ICustomerResolver customerResolver,
        CancellationToken ct = default)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);
        
        var basket = await basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is null)
            return null;

        return new BasketDocument
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            TotalAmount = basket.TotalAmount,
            TotalItems = basket.TotalItems,
            CreatedAt =  basket.CreatedAt,
            ExpiresAt =  basket.ExpiresAt,
            Items = basket.Items.Select(i => new BasketItemDocument
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                VariantId = i.VariantId,
                Sku = i.Sku,
                ImagePath = i.ImagePath,
                Price = i.Price,
                Quantity = i.Quantity,
                StockQuantity = i.StockQuantity,
                TotalPrice = i.TotalPrice,
                AddedAt = i.AddedAt
            }).ToList()
        };
    }
}
