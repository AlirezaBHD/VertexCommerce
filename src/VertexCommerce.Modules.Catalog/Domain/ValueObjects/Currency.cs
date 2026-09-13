using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct Currency
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 3);

    public string Value { get; }

    private Currency(string value)
    {
        Value = value;
    }

    public static Currency Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(Currency)));
}
