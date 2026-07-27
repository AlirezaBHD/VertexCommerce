using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class HeroContentDocument
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

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
    
    [BsonElement("redirectPath")]
    public string RedirectPath { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
