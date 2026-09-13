using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct OptionValue
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private OptionValue(string value)
    {
        Value = value;
    }

    public static OptionValue Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(OptionValue)));
}
