namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Folds Persian and Arabic-Indic digits into their ASCII equivalents.
/// </summary>
/// <remarks>
/// Numeric domain values such as phone numbers and postal codes must be normalized before they are
/// validated or stored: a Persian-digit phone number is a legitimate thing for a user to type, but
/// storing it verbatim would make equality lookups miss. Specs therefore match on <c>[0-9]</c>
/// rather than <c>\d</c>, which in .NET also matches non-ASCII digits.
/// </remarks>
public static class DigitNormalization
{
    private const char PersianZero = '\u06F0';
    private const char PersianNine = '\u06F9';
    private const char ArabicZero = '\u0660';
    private const char ArabicNine = '\u0669';

    public static string ToAsciiDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!ContainsNonAsciiDigit(value))
        {
            return value;
        }

        return string.Create(value.Length, value, static (destination, source) =>
        {
            for (var i = 0; i < source.Length; i++)
            {
                destination[i] = Fold(source[i]);
            }
        });
    }

    private static bool ContainsNonAsciiDigit(string value)
    {
        foreach (var c in value)
        {
            if (Fold(c) != c)
            {
                return true;
            }
        }

        return false;
    }

    private static char Fold(char c) => c switch
    {
        >= PersianZero and <= PersianNine => (char)('0' + (c - PersianZero)),
        >= ArabicZero and <= ArabicNine => (char)('0' + (c - ArabicZero)),
        _ => c
    };
}
