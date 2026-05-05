namespace VertexCommerce.Modules.Basket.Persistence.Documents;

public sealed class BasketItemDocument
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int StockQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ImagePath { get; set; }
    public List<BasketItemAttributeDocument> Attributes { get; set; } = new();
    public DateTime AddedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
