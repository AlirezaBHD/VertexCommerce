using HotChocolate.Data;
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
    [IsProjected(true)]
    public Dictionary<string, List<string>> AvailableOptions { get; set; } = new();
    
    // ═══ Media ═══
    public List<ProductMediaReadModel> Media { get; set; } = [];

    // ═══ Search ═══
    public string SearchText { get; set; } = default!;

    // ═══ Timestamps ═══
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
    
    // ═══ Metadata ═══
    public string Slug { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string? Keywords { get; set; }
}
