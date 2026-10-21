using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;


public readonly record struct PostalCode
{
    public static StringFieldSchema Schema { get; } = new(
        maxLength: 10,
        minLength: 10,
        isUnicode: false,
        pattern: "^[0-9]{10}$");

    public string Value { get; }

    private PostalCode(string value)
    {
        Value = value;
    }

    public static PostalCode Create(string? value)
    {
        var normalized = DigitNormalization.ToAsciiDigits(value ?? string.Empty);
        return new PostalCode(StringFieldGuard.Apply(normalized, Schema, nameof(PostalCode)));
    }
}

