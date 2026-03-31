using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public SeoMetadata Seo { get; private set; } = null!;
    private readonly List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    private Product()
    {
    }

    #region Core Operations

    public static Product Create(
        string name,
        string? description,
        Guid categoryId,
        SeoMetadata seo)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            CategoryId = categoryId,
            Seo = seo,
            IsActive = true
        };

        product.AddDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.Name
        ));

        return product;
    }

    public void Update(string name, string? description, Guid categoryId, SeoMetadata seoMetadata)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim();
        CategoryId = categoryId;
        Seo = seoMetadata;
        SetUpdatedAt();

        AddDomainEvent(new ProductUpdatedEvent(Id, Name));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeCategory(Guid categoryId)
    {
        CategoryId = categoryId;
        SetUpdatedAt();
    }

    public void AddAttribute(string key, string value, string? type = null)
    {
        var existing = _attributes.FirstOrDefault(a => a.Key == key);
        if (existing is not null)
        {
            _attributes.Remove(existing);
        }

        _attributes.Add(ProductAttribute.Create(Id, key, value, type));
        SetUpdatedAt();
    }

    public void RemoveAttribute(string key)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Key == key);
        if (attribute is not null)
        {
            _attributes.Remove(attribute);
            SetUpdatedAt();
        }
    }
    
    public void UpdateAttributes(Dictionary<string, string> commandAttributes)
    {
        var attributes = new List<ProductAttribute>();
        foreach (var commandAttribute in commandAttributes)
        {
            attributes.Add(ProductAttribute.Create(Id, commandAttribute.Key, commandAttribute.Value));
        }
        _attributes.Clear();
        _attributes.AddRange(attributes);
    }

    #endregion
    
    #region Variant Management

    public void AddVariant(ProductVariant variant)
    {
        if (variant.ProductId != Id)
            throw new ArgumentException("Variant does not belong to this product.");

        _variants.Add(variant);
        SetUpdatedAt();
    }

    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is not null)
        {
            _variants.Remove(variant);
            SetUpdatedAt();
        }
    }
    #endregion
    
}
