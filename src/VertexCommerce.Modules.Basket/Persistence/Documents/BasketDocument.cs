using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Basket.Persistence.Documents;

public sealed class BasketDocument
{
    [BsonId]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<BasketItemDocument> Items { get; set; } = [];
    public int TotalItems { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
