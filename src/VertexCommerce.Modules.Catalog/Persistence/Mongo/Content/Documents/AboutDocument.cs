using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("mission")]
    public string? Mission { get; set; }

    [BsonElement("vision")]
    public string? Vision { get; set; }

    [BsonElement("values")]
    public List<AboutValueItem> Values { get; set; } = new();

    [BsonElement("stats")]
    public List<AboutStatItem> Stats { get; set; } = new();

    [BsonElement("team")]
    public List<AboutTeamMember> Team { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public sealed class AboutValueItem
{
    [BsonElement("icon")]
    public string Icon { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class AboutStatItem
{
    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("value")]
    public string Value { get; set; } = string.Empty;

    [BsonElement("suffix")]
    public string? Suffix { get; set; }
}

public sealed class AboutTeamMember
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = string.Empty;

    [BsonElement("bio")]
    public string? Bio { get; set; }

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }
}
