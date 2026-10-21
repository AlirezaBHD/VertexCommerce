namespace VertexCommerce.Shared.Contracts.Baskets;

public sealed record BasketItemDto(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    List<BasketItemAttributeDto> Attributes
);