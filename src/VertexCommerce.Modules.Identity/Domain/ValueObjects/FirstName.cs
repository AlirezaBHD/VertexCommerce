using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Identity.Domain.ValueObjects;

public readonly record struct FirstName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static FirstName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(FirstName)));
}
