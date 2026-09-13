using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct MetaDescription
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 500);

    public string Value { get; }

    private MetaDescription(string value)
    {
        Value = value;
    }

    public static MetaDescription CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(MetaDescription)));
    }
    
    public static MetaDescription Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(MetaDescription)));
}
