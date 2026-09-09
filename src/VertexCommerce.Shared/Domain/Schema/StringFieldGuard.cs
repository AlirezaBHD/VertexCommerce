using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Enforces a <see cref="StringFieldSchema"/> at the domain boundary.
/// This is the only place a spec is turned into an actual runtime check, so every
/// string-backed value object is guaranteed to be validated identically.
/// </summary>
public static class StringFieldGuard
{
    private static readonly ConcurrentDictionary<string, Regex> Patterns = new();

    /// <summary>
    /// Normalizes <paramref name="value"/> (trim) and validates it against <paramref name="schema"/>.
    /// </summary>
    /// <param name="value">The raw incoming value.</param>
    /// <param name="schema">The constraints the value must satisfy.</param>
    /// <param name="field">The domain concept name, used for error reporting.</param>
    /// <returns>The normalized value.</returns>
    /// <exception cref="DomainValidationException">The value violates the schema.</exception>
    public static string Apply(string? value, StringFieldSchema schema, string field)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(field);

        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            if (!schema.AllowEmpty)
            {
                throw new DomainValidationException(field, $"{field} is required.");
            }

            return normalized;
        }

        if (normalized.Length > schema.MaxLength)
        {
            throw new DomainValidationException(
                field, $"{field} must not exceed {schema.MaxLength} characters.");
        }

        if (normalized.Length < schema.MinLength)
        {
            throw new DomainValidationException(
                field, $"{field} must be at least {schema.MinLength} characters.");
        }

        if (schema.Pattern is not null && !Compiled(schema.Pattern).IsMatch(normalized))
        {
            throw new DomainValidationException(field, $"{field} has an invalid format.");
        }

        return normalized;
    }

    private static Regex Compiled(string pattern) =>
        Patterns.GetOrAdd(pattern, static p =>
            new Regex(p, RegexOptions.Compiled | RegexOptions.CultureInvariant));
}
