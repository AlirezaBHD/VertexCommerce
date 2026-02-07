namespace VertexCommerce.Shared.Contracts;

public interface IBasketService
{
    Task<BasketDto?> GetBasketAsync(Guid customerId, CancellationToken ct = default);
    Task ClearBasketAsync(Guid customerId, CancellationToken ct = default);
}

public sealed record BasketDto(
    Guid CustomerId,
    string Currency,
    List<BasketItemDto> Items
);

public sealed record BasketItemDto(
    Guid ProductId,
    string ProductName,
    string? ProductSku,
    decimal UnitPrice,
    int Quantity
);
