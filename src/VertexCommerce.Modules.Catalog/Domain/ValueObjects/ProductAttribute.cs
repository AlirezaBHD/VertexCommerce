using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "In e-commerce catalog domain, 'Attribute' is standard ubiquitous language representing product characteristics, not System.Attribute")]
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
