using VertexCommerce.Modules.Orders.Domain.ValueObjects;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed record GetOrderByIdOrderItemResponse(
    Guid Id,
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string? ProductSku,
    Money UnitPrice,
    int Quantity,
    Money TotalPrice
);