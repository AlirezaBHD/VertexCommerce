using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

/// <summary>The city component of a postal address.</summary>
public readonly record struct City
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private City(string value)
    {
        Value = value;
    }

    public static City Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(City)));
}

