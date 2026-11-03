using System.Text.Json.Serialization;

namespace VertexCommerce.Modules.Catalog.Domain.Banners;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetType
{
    None = 0,
    Product = 1,
    Category = 2,
    InternalPath = 3,
    ExternalUrl = 4
}
