using System.Text.RegularExpressions;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly partial record struct ColorHex
{
    [GeneratedRegex("^#[0-9A-Fa-f]{6}$", RegexOptions.None, matchTimeoutMilliseconds: 100)]
    private static partial Regex Pattern { get; }

    public string Value { get; }

    private ColorHex(string value)
    {
        Value = value;
    }

    public static ColorHex Create(string? value)
    {
        if (value is null || !Pattern.IsMatch(value))
        {
            throw new ArgumentException("Color hex must use the #RRGGBB format.", nameof(value));
        }

        return new ColorHex(value);
    }

    public static ColorHex? CreateOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : Create(value);
}
