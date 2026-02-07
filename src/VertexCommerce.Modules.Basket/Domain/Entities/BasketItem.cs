namespace VertexCommerce.Modules.Basket.Domain.Entities;

public sealed class BasketItem
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string? ProductSku { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = "USD";
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public DateTime AddedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private BasketItem()
    {
    }

    public static BasketItem Create(
        Guid productId,
        string productName,
        string? productSku,
        string? imageUrl,
        decimal unitPrice,
        string currency,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        return new BasketItem
        {
            ProductId = productId,
            ProductName = productName.Trim(),
            ProductSku = productSku?.Trim(),
            ImageUrl = imageUrl?.Trim(),
            UnitPrice = unitPrice,
            Currency = currency.ToUpperInvariant(),
            Quantity = quantity,
            AddedAt = DateTime.UtcNow
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }

        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal unitPrice)
    {
        if (unitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        }

        UnitPrice = unitPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}