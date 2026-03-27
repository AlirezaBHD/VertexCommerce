using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductReadModel
{
    [BsonId]
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    // ═══ Pricing (Aggregated from variants) ═══
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    
    // ═══ Stock ═══
    public int TotalStock { get; set; }
    
    public bool IsActive { get; set; }

    // ═══ Category ═══
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string? CategoryPath { get; set; }

    // ═══ Variants (Embedded) ═══
    public List<ProductVariantReadModel> Variants { get; set; } = [];

    // ═══ Aggregated Options ═══
    // e.g. {"Color": ["Red","Blue"], "Size": ["S","M","L"]}
    public Dictionary<string, List<string>> AvailableOptions { get; set; } = new();
    
    // ═══ Product Attributes ═══
    // e.g. {"Weight": "1.5kg", "Material": "Aluminum"}
    public Dictionary<string, string> Attributes { get; set; } = new();

    // ═══ Search ═══
    public string SearchText { get; set; } = default!;

    // ═══ Timestamps ═══
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
