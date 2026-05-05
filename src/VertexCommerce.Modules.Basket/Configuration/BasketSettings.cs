namespace VertexCommerce.Modules.Basket.Configuration;

public sealed class BasketSettings
{
    public const string SectionName = "Modules:Basket";
    
    public int ExpirationInMinutes { get; init; } = 30;
    
    public int MaxQuantityPerItem { get; init; } = 10;
    
    public int MaxItemsInBasket { get; init; } = 50;
}
