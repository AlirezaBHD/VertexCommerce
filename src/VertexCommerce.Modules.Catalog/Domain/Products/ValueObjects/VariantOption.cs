using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

public sealed class VariantOption : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private VariantOption() { }

    public static VariantOption Create(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Option name cannot be empty.", nameof(name));
        
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Option value cannot be empty.", nameof(value));

        return new VariantOption
        {
            Name = name.Trim(),
            Value = value.Trim()
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
