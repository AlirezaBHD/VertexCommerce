using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct OrderNumber
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 50);

    public string Value { get; }

    private OrderNumber(string value)
    {
        Value = value;
    }

    public static OrderNumber Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(OrderNumber)));
}
