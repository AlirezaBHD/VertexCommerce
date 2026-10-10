using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class CatalogAttributeOption : Entity<Guid>
{
    public Guid AttributeId { get; private set; }
    public OptionCode Code { get; private set; }
    public OptionValue DefaultName { get; private set; }
    public ImagePath? MediaPath { get; private set; }
    public ColorHex? ColorHex { get; private set; }

    private CatalogAttributeOption()
    {
    }

    public static CatalogAttributeOption Create(Guid attributeId, OptionValue defaultName, OptionCode code, ImagePath? mediaPath = null, ColorHex? colorHex = null)
    {
        return new CatalogAttributeOption
        {
            Id = Guid.NewGuid(),
            AttributeId = attributeId,
            DefaultName = defaultName,
            Code = code,
            MediaPath = mediaPath,
            ColorHex = colorHex
        };
    }

    public void Update(Guid attributeId, OptionValue defaultName, OptionCode code, ImagePath? mediaPath = null, ColorHex? colorHex = null)
    {
        AttributeId = attributeId;
        DefaultName = defaultName;
        Code = code;
        MediaPath = mediaPath;
        ColorHex = colorHex;
    }
}