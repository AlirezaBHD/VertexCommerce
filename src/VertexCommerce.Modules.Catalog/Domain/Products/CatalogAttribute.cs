using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class CatalogAttribute : Entity<Guid>
{
    public AttributeCode Code { get; private set; }
    public AttributeName DefaultName { get; private set; }
    public AttributeType? Type { get; private set; }

    private readonly List<CatalogAttributeOption> _options = new();
    public IReadOnlyList<CatalogAttributeOption> Options => _options;

    private CatalogAttribute()
    {
    }

    public static CatalogAttribute Create(AttributeCode code, AttributeName defaultName, AttributeType? type = null)
    {
        if (string.IsNullOrWhiteSpace(code.Value))
        {
            throw new ArgumentException("Catalog attribute cannot be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(defaultName.Value))
        {
            throw new ArgumentException("Catalog attribute default name cannot be empty.", nameof(defaultName));
        }

        return new CatalogAttribute
        {
            Id = Guid.NewGuid(),
            Code = code,
            DefaultName = defaultName,
            Type = type
        };
    }

    public void Update(AttributeCode code, AttributeName defaultName, AttributeType? type = null)
    {
        if (string.IsNullOrWhiteSpace(code.Value))
        {
            throw new ArgumentException("Catalog attribute code cannot be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(defaultName.Value))
        {
            throw new ArgumentException("Catalog attribute default name cannot be empty.", nameof(defaultName));
        }

        DefaultName = defaultName;
        Type = type;
        SetUpdatedAt();
    }

    public void AddOption(OptionValue defaultName, OptionCode optionCode, ImagePath? mediaPath = null)
    {
        _options.Add(CatalogAttributeOption.Create(Id, defaultName, optionCode, mediaPath));
    }
}
