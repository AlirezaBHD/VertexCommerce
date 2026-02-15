namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed record AddItemRequest(
    Guid ProductId,
    int Quantity = 1
);