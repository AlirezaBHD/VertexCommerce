using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.ReadModels;

public sealed class ProductReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Sku { get; set; } = default!;

    public decimal Price { get; set; }
    public string Currency { get; set; } = default!;

    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public bool InStock => StockQuantity > 0;

    // Denormalized Category Data
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string? CategoryPath { get; set; } // "Electronics > Phones > Smartphones"

    // Search optimization
    public string SearchText { get; set; } = default!; // "iphone apple smartphone phone"

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
