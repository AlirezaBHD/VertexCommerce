namespace VertexCommerce.Shared.Contracts.Baskets;

public interface IBasketService
{
    Task<BasketDto?> GetBasketAsync(Guid customerId, CancellationToken ct = default);
    Task ClearBasketAsync(Guid customerId, CancellationToken ct = default);
}