using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct AttributeCode
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private AttributeCode(string value)
    {
        Value = value;
    }

    public static AttributeCode Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(AttributeCode)));
}
