using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct CategoryDescription
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 1000);

    public string Value { get; }

    private CategoryDescription(string value)
    {
        Value = value;
    }

    public static CategoryDescription Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(CategoryDescription)));
}
