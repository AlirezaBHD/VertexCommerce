using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactEmailItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
}
