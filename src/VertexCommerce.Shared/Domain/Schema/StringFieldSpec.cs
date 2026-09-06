using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Defines declarative constraints and shape for a string field.
/// Serves as the single source of truth for both EF Core database mappings and FluentValidation rules.
/// </summary>
/// <param name="MaxLength">
/// The storage ceiling and maximum allowable string length.
/// EF Core: Configures column max length (e.g. VARCHAR(MaxLength)).
/// FluentValidation: Applied via MaximumLength(MaxLength).
/// </param>
/// <param name="MinLength">
/// Validation constraint for minimum string length.
/// EF Core: Ignored (EF has no min-length concept; no check constraint is emitted to prevent schema alterations).
/// FluentValidation: Applied via MinimumLength(MinLength) when &gt; 0.
/// </param>
/// <param name="AllowEmpty">
/// Indicates whether the field can be empty or whitespace-only.
/// EF Core: Ignored. Nullability is strictly inferred from C# Nullable Reference Types (NRT).
/// FluentValidation: If false (default), applied via NotEmpty() which rejects null, empty, and whitespace-only strings.
/// </param>
/// <param name="IsUnicode">
/// Indicates whether the database column supports Unicode characters (e.g. NVARCHAR vs VARCHAR).
/// EF Core: Applied via IsUnicode(IsUnicode).
/// FluentValidation: Ignored.
/// </param>
/// <param name="IsFixedLength">
/// Indicates whether the database column is fixed length (e.g. CHAR/NCHAR vs VARCHAR/NVARCHAR).
/// EF Core: Applied via IsFixedLength(IsFixedLength).
/// FluentValidation: Ignored.
/// </param>
/// <param name="Pattern">
/// Optional regular expression pattern for format validation.
/// EF Core: Ignored.
/// FluentValidation: Applied via Matches(Pattern).
/// </param>
public sealed record StringFieldSpec
{
    public int MaxLength { get; init; }
    public int MinLength { get; init; }
    public bool AllowEmpty { get; init; }
    public bool IsUnicode { get; init; }
    public bool IsFixedLength { get; init; }
    
    [StringSyntax(StringSyntaxAttribute.Regex)] 
    public string? Pattern { get; init; }

    public StringFieldSpec(
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
