using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactStorePhotoItem
{
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("tag")]
    public string Tag { get; set; } = string.Empty;
}
