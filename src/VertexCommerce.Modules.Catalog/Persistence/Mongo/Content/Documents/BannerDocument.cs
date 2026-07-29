using MongoDB.Bson.Serialization.Attributes;
using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class BannerDocument
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("target")]
    public BannerTarget Target { get; set; } = new() { Type = TargetType.None };

    [BsonElement("mediaFileId")]
    public Guid? MediaFileId { get; set; }

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("sortOrder")]
    public int SortOrder { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
