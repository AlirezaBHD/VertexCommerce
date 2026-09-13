using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct AttributeType
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 50);

    public string Value { get; }

    private AttributeType(string value)
    {
        Value = value;
    }

    public static AttributeType CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(AttributeType)));
    }
    
    public static AttributeType Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(AttributeType)));
}
