using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class CatalogAttributeOption : Entity<Guid>
{
    public Guid AttributeId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string DefaultName { get; private set; } = string.Empty;
    public string? MediaPath { get; private set; }

    private CatalogAttributeOption()
    {
    }

    public static CatalogAttributeOption Create(Guid attributeId, string defaultName, string code, string? mediaPath = null)
    {
        if (string.IsNullOrWhiteSpace(defaultName))
        {
            throw new ArgumentException("Catalog attribute option default name cannot be empty.", nameof(defaultName));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Catalog attribute option code cannot be empty.", nameof(code));
        }

        return new CatalogAttributeOption
        {
            Id = Guid.NewGuid(),
            AttributeId = attributeId,
            DefaultName = defaultName,
            Code = code,
            MediaPath = mediaPath
        };
    }

    public void Update(Guid attributeId, string defaultName, string code, string? mediaPath = null)
    {
        if (string.IsNullOrWhiteSpace(defaultName))
        {
            throw new ArgumentException("Catalog attribute option default name cannot be empty.", nameof(defaultName));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Catalog attribute option code cannot be empty.", nameof(code));
        }

        AttributeId = attributeId;
        DefaultName = defaultName;
        Code = code;
        MediaPath = mediaPath;
    }
}
