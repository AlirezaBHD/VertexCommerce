using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutQualitySection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("paragraphs")]
    public List<string> Paragraphs { get; set; } = new();

    [BsonElement("features")]
    public List<AboutQualityFeatureItem> Features { get; set; } = new();

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("imageBadgeTitle")]
    public string? ImageBadgeTitle { get; set; }

    [BsonElement("imageBadgeSubtitle")]
    public string? ImageBadgeSubtitle { get; set; }
}
