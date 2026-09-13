using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct CategoryName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private CategoryName(string value)
    {
        Value = value;
    }

    public static CategoryName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(CategoryName)));
}
