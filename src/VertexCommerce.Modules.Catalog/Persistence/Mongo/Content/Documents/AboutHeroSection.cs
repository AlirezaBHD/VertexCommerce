using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutHeroSection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("buttonText")]
    public string ButtonText { get; set; } = string.Empty;

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("showCat")]
    public bool ShowCat { get; set; } = true;

    [BsonElement("catImagePath")]
    public string? CatImagePath { get; set; }
}
