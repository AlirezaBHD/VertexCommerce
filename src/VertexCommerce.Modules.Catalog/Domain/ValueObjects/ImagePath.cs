using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct ImagePath
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 500);

    public string Value { get; }

    private ImagePath(string value)
    {
        Value = value;
    }

    public static ImagePath CreateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return new(StringFieldGuard.Apply(value, Schema, nameof(ImagePath)));
    }
    
    public static ImagePath Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(ImagePath)));
}
