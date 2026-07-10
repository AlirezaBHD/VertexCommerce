using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000002");

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("workingHours")]
    public string? WorkingHours { get; set; }

    [BsonElement("mapEmbedUrl")]
    public string? MapEmbedUrl { get; set; }

    [BsonElement("socialLinks")]
    public List<SocialLinkItem> SocialLinks { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public sealed class SocialLinkItem
{
    [BsonElement("platform")]
    public string Platform { get; set; } = string.Empty;

    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("icon")]
    public string Icon { get; set; } = string.Empty;
}
