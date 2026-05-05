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
    
    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    
    private readonly List<ProductMedia> _media = [];
    public IReadOnlyCollection<ProductMedia> Media => _media.AsReadOnly();

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

    public void Delete()
    {
        AddDomainEvent(new ProductDeletedEvent(Id));
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

    #region Media Management

    public void AddMedia(ProductMedia media)
    {
        _media.Add(media);
        SetUpdatedAt();
    }
    public void SetMedia(IEnumerable<ProductMedia> mediaList)
    {
        _media.Clear();
        _media.AddRange(mediaList);
        SetUpdatedAt();
    }
    
    public void ReplaceMedia(List<ProductMedia> newMedias)
    {
        _media.RemoveAll(m => !newMedias.Any(newM => newM.Path == m.Path));
        var toAdd = newMedias.Where(newM => !_media.Any(m => m.Path == newM.Path));
        _media.AddRange(toAdd);
    
        var sorted = _media.OrderBy(m => m.SortOrder).ToList();
        _media.Clear();
        _media.AddRange(sorted);

        SetUpdatedAt();
    }

    public void RemoveMedia(string path)
    {
        var media = _media.FirstOrDefault(m => m.Path == path);
        if (media is not null)
        {
            _media.Remove(media);
            SetUpdatedAt();
        }
    }

    #endregion
}
