using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Identity.Domain.ValueObjects;

/// <summary>
/// A phone number formatted in international E.164 form (e.g. <c>+989XXXXXXXXX</c>, <c>+12025550123</c>).
/// Automatically normalizes Iranian local numbers (<c>09XXXXXXXXX</c>) to international format (<c>+989XXXXXXXXX</c>).
/// </summary>
public readonly record struct PhoneNumber
{
    public static StringFieldSchema Schema { get; } = new(
        maxLength: 20,
        minLength: 8,
        isUnicode: false,
        pattern: @"^\+[1-9][0-9]{6,14}$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string? value)
    {
        var normalized = Normalize(value);
        return new PhoneNumber(StringFieldGuard.Apply(normalized, Schema, nameof(PhoneNumber)));
    }

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var ascii = DigitNormalization.ToAsciiDigits(value.Trim());
        var cleaned = ascii.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (cleaned.StartsWith('+'))
        {
            return cleaned;
        }

        if (cleaned.StartsWith("00", StringComparison.Ordinal))
        {
            return "+" + cleaned[2..];
        }

        if (cleaned.StartsWith("09", StringComparison.Ordinal) && cleaned.Length == 11)
        {
            return "+98" + cleaned[1..];
        }

        if (cleaned.StartsWith("989", StringComparison.Ordinal) && cleaned.Length == 12)
        {
            return "+" + cleaned;
        }

        if (cleaned.StartsWith('9') && cleaned.Length == 10)
        {
            return "+98" + cleaned;
        }

        return cleaned;
    }
}
