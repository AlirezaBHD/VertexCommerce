using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutStorySection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("paragraphs")]
    public List<string> Paragraphs { get; set; } = new();

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("imageBadge")]
    public string? ImageBadge { get; set; }

    [BsonElement("supportText")]
    public string? SupportText { get; set; }
}
