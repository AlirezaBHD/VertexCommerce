using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct AltText
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 200);

    public string Value { get; }

    private AltText(string value)
    {
        Value = value;
    }

    public static AltText CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(AltText)));
    }
    
    public static AltText Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(AltText)));
}
