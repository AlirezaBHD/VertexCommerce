using VertexCommerce.Modules.Basket.Domain.Entities;
using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.GetBasket;

public sealed class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, BasketResponse>
{
    private readonly IBasketRepository _basketRepository;

    public GetBasketQueryHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<BasketResponse>> Handle(GetBasketQuery query, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(query.CustomerId, ct);

        // If basket doesn't exist, return empty basket
        if (basket is null)
        {
            basket = CustomerBasket.Create(query.CustomerId);
        }

        var response = MapToResponse(basket);

        return Result.Success(response);
    }

    private static BasketResponse MapToResponse(CustomerBasket basket)
    {
        return new BasketResponse(
            basket.Id,
            basket.CustomerId,
            basket.Items.Select(i => new BasketItemResponse(
                i.ProductId,
                i.ProductName,
                i.ProductSku,
                i.ImageUrl,
                i.UnitPrice,
                i.Quantity,
                i.TotalPrice,
                i.AddedAt
            )).ToList(),
            basket.Currency,
            basket.TotalAmount,
            basket.TotalItems,
            basket.CreatedAt,
            basket.UpdatedAt,
            basket.ExpiresAt
        );
    }
}
