using VertexCommerce.Modules.Catalog.Domain.Events;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Sku Sku { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

    private Product()
    {
    }

    public static Product Create(
        string name,
        string? description,
        Sku sku,
        Money price,
        int stockQuantity,
        Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Sku = sku,
            Price = price,
            StockQuantity = stockQuantity,
            CategoryId = categoryId,
            IsActive = true
        };

        product.AddDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Sku.Value,
            product.Price.Amount,
            product.Price.Currency
        ));

        return product;
    }

    public void Update(string name, string? description, Money price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        CategoryId = categoryId;
        SetUpdatedAt();

        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Price.Amount));
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(quantity));
    
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
    
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
    
        if (StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock");
    
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
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
    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(quantity));
        }

        StockQuantity = quantity;
        SetUpdatedAt();
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
}
