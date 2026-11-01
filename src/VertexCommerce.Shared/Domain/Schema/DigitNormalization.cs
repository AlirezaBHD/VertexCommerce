namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Folds Persian and Arabic-Indic digits into their ASCII equivalents.
/// </summary>
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
        return value.Any(c => Fold(c) != c);
    }

    private static char Fold(char c) => c switch
    {
        >= PersianZero and <= PersianNine => (char)('0' + (c - PersianZero)),
        >= ArabicZero and <= ArabicNine => (char)('0' + (c - ArabicZero)),
        _ => c
    };
}
