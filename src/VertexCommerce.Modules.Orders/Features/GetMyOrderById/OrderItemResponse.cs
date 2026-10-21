namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string? ProductSku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);