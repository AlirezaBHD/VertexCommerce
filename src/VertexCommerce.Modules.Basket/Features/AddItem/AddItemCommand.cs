using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed record AddItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity = 1
) : ICommand;