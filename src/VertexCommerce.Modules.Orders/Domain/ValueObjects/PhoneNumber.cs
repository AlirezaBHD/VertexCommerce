using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

/// <summary>
/// An Iranian mobile number, normalized to ASCII digits in <c>09XXXXXXXXX</c> form.
/// </summary>
public readonly record struct PhoneNumber
{
    /// <remarks>
    /// The column keeps its historical width of 20 while the pattern pins the value to exactly 11
    /// ASCII digits, so tightening the format never requires a schema change.
    /// </remarks>
    public static StringFieldSchema Schema { get; } = new(
        maxLength: 20,
        minLength: 11,
        isUnicode: false,
        pattern: "^09[0-9]{9}$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string? value)
    {
        var normalized = DigitNormalization.ToAsciiDigits(value ?? string.Empty);
        return new PhoneNumber(StringFieldGuard.Apply(normalized, Schema, nameof(PhoneNumber)));
    }
}

