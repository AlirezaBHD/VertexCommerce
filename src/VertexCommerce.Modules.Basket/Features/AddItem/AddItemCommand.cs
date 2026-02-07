using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

public sealed record AddItemCommand(
    Guid CustomerId,
    Guid ProductId,
    string ProductName,
    string? ProductSku,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    string Currency = "USD"
) : ICommand;
