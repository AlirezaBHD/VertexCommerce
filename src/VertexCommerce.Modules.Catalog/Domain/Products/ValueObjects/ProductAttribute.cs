using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

public sealed class ProductAttribute : ValueObject
{
    public string AttributeCode { get; private set; } = string.Empty;
    public string OptionCode { get; private set; } = string.Empty;

    private ProductAttribute() { }

    public static ProductAttribute Create(string attributeCode, string optionCode)
    {
        if (string.IsNullOrWhiteSpace(attributeCode))
            throw new ArgumentException("Product attribute attribute code cannot be empty.", nameof(attributeCode));
        
        if (string.IsNullOrWhiteSpace(optionCode))
            throw new ArgumentException("Product attribute option code cannot be empty.", nameof(optionCode));

        return new ProductAttribute
        {
            AttributeCode = attributeCode.Trim(),
            OptionCode = optionCode.Trim()   
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AttributeCode;
        yield return OptionCode;
    }
}