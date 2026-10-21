using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct Province
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private Province(string value)
    {
        Value = value;
    }

    public static Province Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(Province)));
}

