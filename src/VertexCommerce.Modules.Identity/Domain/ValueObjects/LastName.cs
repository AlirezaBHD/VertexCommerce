using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Identity.Domain.ValueObjects;

public readonly record struct LastName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static LastName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(LastName)));
}
