using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed record AddItemCommand(
    Guid ProductId,
    Guid VariantId,
    int Quantity = 1
) : ICommand;