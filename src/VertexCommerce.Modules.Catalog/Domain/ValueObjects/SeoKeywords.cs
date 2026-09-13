using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct SeoKeywords
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 200);

    public string Value { get; }

    private SeoKeywords(string value)
    {
        Value = value;
    }

    public static SeoKeywords CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(SeoKeywords)));
    }
    
    public static SeoKeywords Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(SeoKeywords)));
}
