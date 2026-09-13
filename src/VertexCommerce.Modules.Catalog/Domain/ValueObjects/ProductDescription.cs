using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct ProductDescription
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 2000);

    public string Value { get; }

    private ProductDescription(string value)
    {
        Value = value;
    }

    public static ProductDescription CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(ProductDescription)));
    }
    
    public static ProductDescription Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(ProductDescription)));
}