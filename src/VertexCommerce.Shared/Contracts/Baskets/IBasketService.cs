namespace VertexCommerce.Shared.Contracts.Baskets;

public interface IBasketService
{
    Task<BasketDto?> GetBasketAsync(Guid customerId, CancellationToken ct = default);
    Task ClearBasketAsync(Guid customerId, CancellationToken ct = default);
}

public sealed record BasketDto(
    Guid Id,
    Guid CustomerId,
    List<BasketItemDto> Items
);

public sealed record BasketItemDto(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    List<BasketItemAttributeDto> Attributes
);

public sealed record BasketItemAttributeDto(
    string AttributeCode, string OptionCode);
