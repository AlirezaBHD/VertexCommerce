using System.Text.RegularExpressions;
using VertexCommerce.Shared.Domain.Schema;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly partial record struct Sku
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 50);

    public string Value { get; }

    private Sku(string value)
    {
        Value = value;
    }

    public static Sku Create(string? value)
    {
        var val = StringFieldGuard.Apply(value, Schema, nameof(Sku));
        var normalizedValue = val.Trim().ToUpperInvariant();

        if (normalizedValue.Length < 3)
            throw new DomainValidationException("Catalog.Sku", "SKU must be at least 3 characters.");

        if (!SkuRegex().IsMatch(normalizedValue))
            throw new DomainValidationException("Catalog.Sku", "SKU can only contain letters, numbers, and hyphens.");

        return new Sku(normalizedValue);
    }

    public static Sku Generate(string prefix = "PRD")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return new Sku($"--");
    }

    [GeneratedRegex(@"^[A-Z0-9\-]+$")]
    private static partial Regex SkuRegex();
}
