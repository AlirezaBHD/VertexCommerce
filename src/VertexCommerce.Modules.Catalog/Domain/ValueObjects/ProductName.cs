using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct ProductName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 200);

    public string Value { get; }

    private ProductName(string value)
    {
        Value = value;
    }

    public static ProductName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(ProductName)));
}
