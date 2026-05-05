using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(
    Guid ProductId,
    Guid VariantId,
    int Quantity
) : ICommand;