using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactPhoneItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("number")]
    public string Number { get; set; } = string.Empty;

    [BsonElement("raw")]
    public string Raw { get; set; } = string.Empty;

    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
}
