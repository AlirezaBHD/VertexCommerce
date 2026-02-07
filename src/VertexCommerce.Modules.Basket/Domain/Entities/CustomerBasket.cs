namespace VertexCommerce.Modules.Basket.Domain.Entities;

public sealed class CustomerBasket
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public List<BasketItem> Items { get; private set; } = [];
    public string Currency { get; private set; } = "USD";
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    public int TotalItems => Items.Sum(i => i.Quantity);
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private CustomerBasket()
    {
    }

    public static CustomerBasket Create(Guid customerId, string currency = "USD", int expirationDays = 30)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        }

        return new CustomerBasket
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Currency = currency.ToUpperInvariant(),
            Items = [],
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays)
        };
    }

    public void AddItem(
        Guid productId,
        string productName,
        string? productSku,
        string? imageUrl,
        decimal unitPrice,
        int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var item = BasketItem.Create(
                productId,
                productName,
                productSku,
                imageUrl,
                unitPrice,
                Currency,
                quantity
            );
            Items.Add(item);
        }

        UpdatedAt = DateTime.UtcNow;
        ExtendExpiration();
    }

    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is not null)
        {
            Items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException($"Item with product ID '{productId}' not found in basket.");
        }

        if (quantity <= 0)
        {
            Items.Remove(item);
        }
        else
        {
            item.UpdateQuantity(quantity);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmpty => Items.Count == 0;

    public bool HasItem(Guid productId) => Items.Any(i => i.ProductId == productId);

    public BasketItem? GetItem(Guid productId) => Items.FirstOrDefault(i => i.ProductId == productId);

    private void ExtendExpiration(int days = 30)
    {
        ExpiresAt = DateTime.UtcNow.AddDays(days);
    }
}