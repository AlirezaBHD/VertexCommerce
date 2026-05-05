using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

public sealed class CategoryReadModel
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("parentId")]
    public Guid? ParentId { get; set; }
    
    [BsonElement("iconPath")]
    public string? IconPath { get; set; }
    
    [BsonElement("coverImagePath")]
    public string CoverImagePath { get; set; } = string.Empty;
    
    [BsonElement("imageAltText")]
    public string? ImageAltText { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
    
    [BsonElement("showOnHome")]
    public bool ShowOnHome { get; set; }
    
    [BsonElement("includeInMenu")]
    public bool IncludeInMenu { get; set; }

    [BsonElement("sortOrder")]
    public int SortOrder { get; set; }

    // === Denormalized Path ===
    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;
    // e.g. "Electronics > Phones > Samsung"

    [BsonElement("pathIds")]
    public List<Guid> PathIds { get; set; } = [];
    // e.g. [rootId, parentId, thisId]

    [BsonElement("depth")]
    public int Depth { get; set; }
    // 0 = root, 1 = child, 2 = grandchild

    // === Children Summary ===
    [BsonElement("childCount")]
    public int ChildCount { get; set; }

    [BsonElement("productCount")]
    public int ProductCount { get; set; }

    // === Timestamps ===
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    // ═══ Metadata ═══
    [BsonElement("slug")]
    public string Slug { get; set; } = string.Empty;

    [BsonElement("metaTitle")]
    public string MetaTitle { get; set; } = string.Empty;

    [BsonElement("metaDescription")]
    public string MetaDescription { get; set; } = string.Empty;

    [BsonElement("keywords")]
    public string? Keywords { get; set; }
}
