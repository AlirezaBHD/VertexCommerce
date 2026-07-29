using System.Text.Json.Serialization;

namespace VertexCommerce.Modules.Catalog.Domain.Banners;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetType
{
    None,
    Product,
    Category,
    InternalPath,
    ExternalUrl
}
