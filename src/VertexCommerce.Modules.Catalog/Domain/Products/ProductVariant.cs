using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class ProductVariant : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Sku Sku { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public int Order { get; private set; }

    private readonly List<VariantOption> _options = [];
    public IReadOnlyCollection<VariantOption> Options => _options.AsReadOnly();

    private readonly List<ProductMedia> _media = [];
    public IReadOnlyCollection<ProductMedia> Media => _media.AsReadOnly();

    private ProductVariant() { }

    public static ProductVariant Create(
        Guid productId,
        Sku sku,
        List<VariantOption> options,
        int stockQuantity,
        int order,
        Money price)
    {
        if (!options.Any())
            throw new ArgumentException("Variant must have at least one option.", nameof(options));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(stockQuantity));

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Sku = sku,
            Price = price,
            StockQuantity = stockQuantity,
            IsActive = true,
            Order = order
        };

        variant._options.AddRange(options);
        return variant;
    }

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
    public void RemoveMedia(string path)
    {
        var media = _media.FirstOrDefault(m => m.Path == path);
        if (media is not null)
        {
            _media.Remove(media);
            SetUpdatedAt();
        }
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(quantity));

        StockQuantity = quantity;
        SetUpdatedAt();
    }

    public void SetPrice(Money price)
    {
        Price = price;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
