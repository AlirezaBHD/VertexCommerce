using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct AttributeName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private AttributeName(string value)
    {
        Value = value;
    }

    public static AttributeName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(AttributeName)));
}
