using MongoDB.Bson.Serialization.Attributes;
using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class HeroContentDocument
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("target")]
    public BannerTarget Target { get; set; } = new() { Type = TargetType.None };

    [BsonElement("imageMediaFileId")]
    public Guid? ImageMediaFileId { get; set; }

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("mobileImageMediaFileId")]
    public Guid? MobileImageMediaFileId { get; set; }

    [BsonElement("mobileImagePath")]
    public string? MobileImagePath { get; set; }

    [BsonElement("videoMediaFileId")]
    public Guid? VideoMediaFileId { get; set; }
    
    [BsonElement("videoPath")]
    public string? VideoPath { get; set; }
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
