using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Shared.Domain.Schema;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class StringFieldAttribute(int maxLength) : MaxLengthAttribute(maxLength)
{
    public int MinLength { get; init; }
    
    /// <summary>
    /// If true, bypasses NotEmpty() validation, allowing empty strings or whitespace.
    /// Note: Nullability is exclusively driven by C# NRTs (e.g. string vs string?).
    /// </summary>
    public bool AllowEmpty { get; init; }
    
    public bool Ascii { get; init; }
    public bool FixedLength { get; init; }

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public string? Pattern { get; init; }

    public StringFieldSpec ToSpec() =>
        new(
            maxLength: Length,
            minLength: MinLength,
            allowEmpty: AllowEmpty,
            isUnicode: !Ascii,
            isFixedLength: FixedLength,
            pattern: Pattern);
}
