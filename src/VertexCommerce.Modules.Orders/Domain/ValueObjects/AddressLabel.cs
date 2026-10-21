using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct AddressLabel
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 50, allowEmpty: true);

    public string Value { get; }

    private AddressLabel(string value)
    {
        Value = value;
    }
    
    public static AddressLabel? CreateOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : new AddressLabel(StringFieldGuard.Apply(value, Schema, nameof(AddressLabel)));
}

