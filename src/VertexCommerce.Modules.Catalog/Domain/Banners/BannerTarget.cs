using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Domain.Banners;

public sealed class BannerTarget
{
    [BsonElement("type")]
    public TargetType Type { get; set; }

    [BsonElement("productId")]
    public Guid? ProductId { get; set; }

    [BsonElement("productTitleSnapshot")]
    public string? ProductTitleSnapshot { get; set; }

    [BsonElement("productSlugSnapshot")]
    public string? ProductSlugSnapshot { get; set; }

    [BsonElement("productSkuSnapshot")]
    public string? ProductSkuSnapshot { get; set; }

    [BsonElement("categoryId")]
    public Guid? CategoryId { get; set; }

    [BsonElement("categoryTitleSnapshot")]
    public string? CategoryTitleSnapshot { get; set; }

    [BsonElement("categorySlugSnapshot")]
    public string? CategorySlugSnapshot { get; set; }

    [BsonElement("internalPath")]
    public string? InternalPath { get; set; }

    [BsonElement("externalUrl")]
    public string? ExternalUrl { get; set; }
}
