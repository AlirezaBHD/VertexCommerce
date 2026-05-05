namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed record AddItemRequest(
    Guid VariantId,
    Guid ProductId,
    int Quantity = 1
);