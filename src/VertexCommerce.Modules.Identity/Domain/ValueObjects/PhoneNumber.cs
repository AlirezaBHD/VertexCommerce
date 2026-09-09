using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Identity.Domain.ValueObjects;

public readonly record struct PhoneNumber
{
    public static StringFieldSchema Schema { get; } = new(
        maxLength: 256,
        pattern: @"^\+?[1-9]\d{1,14}$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(PhoneNumber)));
}
