namespace VertexCommerce.Shared.Contracts.Baskets;

public sealed record BasketDto(
    Guid Id,
    Guid CustomerId,
    List<BasketItemDto> Items
);