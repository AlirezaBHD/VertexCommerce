using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class WorkingHourScheduleItem
{
    [BsonElement("day")]
    public string Day { get; set; } = string.Empty;

    [BsonElement("time")]
    public string Time { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}
