namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public sealed record ProductAttribute
{
    public AttributeCode AttributeCode { get; }
    public OptionCode OptionCode { get; }

    private ProductAttribute(AttributeCode attributeCode, OptionCode optionCode)
    {
        AttributeCode = attributeCode;
        OptionCode = optionCode;
    }

    public static ProductAttribute Create(AttributeCode attributeCode, OptionCode optionCode)
    {
        return new ProductAttribute(attributeCode, optionCode);
    }
}
