using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Basket.Persistence.Documents;

public sealed class BasketDocument
{
    [BsonId]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<BasketItemDocument> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public BasketItemDocument? FindItem(Guid productId, Guid variantId) =>
        Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);

    public BasketItemDocument? FindItem(Guid variantId) =>
        Items.FirstOrDefault(i => i.VariantId == variantId);

    public void RemoveItem(Guid variantId)
    {
        var item = FindItem(variantId);

        if (item is not null)
        {
            Items.Remove(item);
            RecalculateTotals();
        }
    }

    public void RecalculateTotals()
    {
        TotalItems = Items.Sum(i => i.Quantity);
        TotalAmount = Items.Sum(i => i.Price * i.Quantity);
    }
}
