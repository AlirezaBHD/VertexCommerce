using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.ClearBasket;

public sealed record ClearBasketCommand(Guid CustomerId) : ICommand;
