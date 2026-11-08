using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutCtaSection
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("buttonText")]
    public string ButtonText { get; set; } = string.Empty;

    [BsonElement("buttonLink")]
    public string ButtonLink { get; set; } = string.Empty;
}
