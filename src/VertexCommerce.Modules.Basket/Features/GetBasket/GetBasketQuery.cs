using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.GetBasket;

public sealed record GetBasketQuery(Guid CustomerId) : IQuery<BasketResponse>;
