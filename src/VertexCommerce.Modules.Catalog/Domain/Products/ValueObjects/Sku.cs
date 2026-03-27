using System.Text.RegularExpressions;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

public sealed partial class Sku : ValueObject
{
    public string Value { get; private set; }

    private Sku()
    {
        Value = string.Empty;
    }

    private Sku(string value)
    {
        Value = value;
    }

    public static Sku Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("SKU cannot be empty.", nameof(value));
        }

        var normalizedValue = value.Trim().ToUpperInvariant();

        if (normalizedValue.Length < 3 || normalizedValue.Length > 50)
        {
            throw new ArgumentException("SKU must be between 3 and 50 characters.", nameof(value));
        }

        if (!SkuRegex().IsMatch(normalizedValue))
        {
            throw new ArgumentException("SKU can only contain letters, numbers, and hyphens.", nameof(value));
        }

        return new Sku(normalizedValue);
    }

    public static Sku Generate(string prefix = "PRD")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return new Sku($"{prefix}-{timestamp}-{random}");
    }

    [GeneratedRegex(@"^[A-Z0-9\-]+$")]
    private static partial Regex SkuRegex();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.Value;
}
