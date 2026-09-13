using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public enum MediaType
{
    Image = 1,
    Video = 2
}

public sealed record ProductMedia
{
    public ImagePath Path { get; }
    public MediaType Type { get; }
    public int SortOrder { get; }
    public AltText? AltText { get; }
    public AttributeCode? AssociatedAttributeCode { get; }
    public OptionCode? AssociatedOptionCode { get; }

    private ProductMedia(ImagePath path, MediaType type, int sortOrder, AltText? altText, AttributeCode? associatedAttributeCode, OptionCode? associatedOptionCode)
    {
        Path = path;
        Type = type;
        SortOrder = sortOrder;
        AltText = altText;
        AssociatedAttributeCode = associatedAttributeCode;
        AssociatedOptionCode = associatedOptionCode;
    }

    public static ProductMedia Create(ImagePath path, MediaType type, int order = 0, AltText? altText = null, AttributeCode? associatedAttributeCode = null, OptionCode? associatedOptionCode = null)
    {
        return new ProductMedia(path, type, order, altText, associatedAttributeCode, associatedOptionCode);
    }
}