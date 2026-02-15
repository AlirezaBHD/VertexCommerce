using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity
) : ICommand;