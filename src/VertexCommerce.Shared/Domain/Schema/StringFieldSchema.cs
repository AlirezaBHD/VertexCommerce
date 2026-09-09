using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Declarative constraints and storage shape for a string-backed domain concept.
/// </summary>
/// <remarks>
/// A spec is declared once, as a static member of the value object that owns the concept,
/// and is the single source of truth for three consumers:
/// <list type="bullet">
/// <item>the domain guard, via <see cref="StringFieldGuard"/>, which makes an invalid instance unconstructable;</item>
/// <item>the EF Core column mapping, via <c>HasSchema</c> in the entity configuration;</item>
/// <item>the FluentValidation rules, via <c>HasSchema</c> in the command validator.</item>
/// </list>
/// </remarks>
public sealed record StringFieldSchema
{
    /// <summary>
    /// Maximum allowed length. Drives the column length and the validator's <c>MaximumLength</c>.
    /// </summary>
    public int MaxLength { get; init; }

    /// <summary>
    /// Minimum allowed length. Drives the validator's <c>MinimumLength</c> when greater than zero.
    /// Not expressible in EF Core, which has no min-length concept, so no check constraint is emitted.
    /// </summary>
    public int MinLength { get; init; }

    /// <summary>
    /// Whether the value may be empty or whitespace-only. When <c>false</c> (the default) both the
    /// domain guard and the validator reject blank input. Independent of nullability, which is
    /// driven exclusively by C# nullable reference types.
    /// </summary>
    public bool AllowEmpty { get; init; }

    /// <summary>
    /// Whether the column stores Unicode (<c>nvarchar</c>/<c>text</c>) rather than ASCII (<c>varchar</c>).
    /// Storage-only; the domain guard ignores it.
    /// </summary>
    public bool IsUnicode { get; init; }

    /// <summary>
    /// Whether the column is fixed length (<c>char</c> rather than <c>varchar</c>).
    /// Storage-only; the domain guard ignores it.
    /// </summary>
    public bool IsFixedLength { get; init; }

    /// <summary>
    /// Optional format constraint, enforced by both the domain guard and the validator.
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public string? Pattern { get; init; }

    public StringFieldSchema(
        int maxLength,
        int minLength = 0,
        bool allowEmpty = false,
        bool isUnicode = true,
        bool isFixedLength = false,
        [StringSyntax(StringSyntaxAttribute.Regex)] string? pattern = null)
    {
        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "MaxLength must be greater than zero.");

        if (minLength < 0 || minLength > maxLength)
            throw new ArgumentOutOfRangeException(nameof(minLength), "MinLength must be between 0 and MaxLength.");

        MaxLength = maxLength;
        MinLength = minLength;
        AllowEmpty = allowEmpty;
        IsUnicode = isUnicode;
        IsFixedLength = isFixedLength;
        Pattern = pattern;
    }
}
