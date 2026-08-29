using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class ProductVariant : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Sku Sku { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => StockQuantity - ReservedQuantity;
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<ProductAttribute> _attributes = [];
    public IReadOnlyList<ProductAttribute> Attributes => _attributes.AsReadOnly();

    private ProductVariant() { }

    public static ProductVariant Create(
        Guid productId,
        Sku sku,
        int stockQuantity,
        int order,
        Money price,
        List<ProductAttribute> attributes)
    {
        if (stockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(stockQuantity));

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Sku = sku,
            Price = price,
            StockQuantity = stockQuantity,
            ReservedQuantity = 0,
            IsActive = true,
            SortOrder = order};

        variant._attributes.AddRange(attributes);
        return variant;
    }

    public void Update(
        Sku sku,
        int stockQuantity,
        int order,
        Money price,
        List<ProductAttribute> attributes)
    {
        if (Sku.Value != sku.Value) Sku = sku;
        if (Price.Amount != price.Amount || Price.Currency != price.Currency) Price = price;
    
        StockQuantity = stockQuantity;
        SortOrder = order;

        _attributes.RemoveAll(o => !_attributes.Any(newO => newO.AttributeCode == o.AttributeCode && newO.OptionCode == o.OptionCode));
        var newOptions = _attributes.Where(newO => !_attributes.Any(o => o.AttributeCode == newO.AttributeCode && o.OptionCode == newO.OptionCode));
        _attributes.AddRange(newOptions);
    
        SetUpdatedAt();
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(quantity));

        StockQuantity = quantity;
        SetUpdatedAt();
    }

    public bool TryReserveStock(int quantity)
    {
        if (quantity <= 0 || AvailableQuantity < quantity)
            return false;

        ReservedQuantity += quantity;
        SetUpdatedAt();
        return true;
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0) return;
        
        ReservedQuantity -= quantity;
        if (ReservedQuantity < 0) ReservedQuantity = 0;
        
        SetUpdatedAt();
    }

    public bool TryCommitReservedStock(int quantity)
    {
        if (quantity <= 0 || ReservedQuantity < quantity || StockQuantity < quantity)
            return false;

        ReservedQuantity -= quantity;
        StockQuantity -= quantity;
        SetUpdatedAt();
        return true;
    }

    public bool TryDeductStock(int quantity)
    {
        if (quantity <= 0 || AvailableQuantity < quantity)
            return false;

        StockQuantity -= quantity;
        SetUpdatedAt();
        return true;
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
