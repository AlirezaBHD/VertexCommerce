using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct ColorHex
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 7);

    public string Value { get; }

    private ColorHex(string value)
    {
        Value = value;
    }

    public static ColorHex CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(ColorHex)));
    }
    
    public static ColorHex Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(ColorHex)));
}
