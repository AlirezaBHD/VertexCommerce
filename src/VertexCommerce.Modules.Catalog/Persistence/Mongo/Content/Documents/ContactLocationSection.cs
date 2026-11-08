using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactLocationSection
{
    [BsonElement("addressText")]
    public string AddressText { get; set; } = string.Empty;

    [BsonElement("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [BsonElement("mapImagePath")]
    public string? MapImagePath { get; set; }

    [BsonElement("mapTitle")]
    public string MapTitle { get; set; } = string.Empty;

    [BsonElement("mapSubtitle")]
    public string MapSubtitle { get; set; } = string.Empty;

    [BsonElement("neshanUrl")]
    public string? NeshanUrl { get; set; }

    [BsonElement("baladUrl")]
    public string? BaladUrl { get; set; }

    [BsonElement("googleMapsUrl")]
    public string? GoogleMapsUrl { get; set; }
}
