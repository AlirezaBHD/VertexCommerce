namespace VertexCommerce.Modules.Basket.Features.GetBasket;

public sealed record BasketResponse(
    Guid Id,
    Guid CustomerId,
    List<BasketItemResponse> Items,
    string Currency,
    decimal TotalAmount,
    int TotalItems,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ExpiresAt
);

public sealed record BasketItemResponse(
    Guid ProductId,
    string ProductName,
    string? ProductSku,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    DateTime AddedAt
);
