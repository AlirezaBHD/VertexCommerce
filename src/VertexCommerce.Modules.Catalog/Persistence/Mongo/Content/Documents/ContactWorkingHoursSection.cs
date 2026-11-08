using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactWorkingHoursSection
{
    [BsonElement("isOpenNow")]
    public bool IsOpenNow { get; set; } = true;

    [BsonElement("items")]
    public List<WorkingHourScheduleItem> Items { get; set; } = new();
}
