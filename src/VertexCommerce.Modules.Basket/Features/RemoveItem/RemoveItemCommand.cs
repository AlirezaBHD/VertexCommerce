using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.RemoveItem;

public sealed record RemoveItemCommand(
    Guid CustomerId,
    Guid ProductId
) : ICommand;
