namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

public sealed record UpdateItemQuantityRequest(
    Guid VariantId,
    int Quantity
);