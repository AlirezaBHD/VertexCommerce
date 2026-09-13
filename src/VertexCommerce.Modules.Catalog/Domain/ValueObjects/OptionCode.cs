using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct OptionCode
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private OptionCode(string value)
    {
        Value = value;
    }

    public static OptionCode Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(OptionCode)));
}
