using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct Slug
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 255);

    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Slug Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(Slug)));
}
