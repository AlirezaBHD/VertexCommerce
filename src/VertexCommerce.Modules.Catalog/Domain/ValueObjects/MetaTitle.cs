using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct MetaTitle
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 150);

    public string Value { get; }

    private MetaTitle(string value)
    {
        Value = value;
    }

    public static MetaTitle CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(MetaTitle)));
    }
    
    public static MetaTitle Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(MetaTitle)));
}
