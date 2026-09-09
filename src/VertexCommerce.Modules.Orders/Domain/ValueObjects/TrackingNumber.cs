using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct TrackingNumber
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private TrackingNumber(string value)
    {
        Value = value;
    }

    public static TrackingNumber Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(TrackingNumber)));
}
