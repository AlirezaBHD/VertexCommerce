using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string? ProductSku { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money TotalPrice => UnitPrice.Multiply(Quantity);

    private OrderItem()
    {
    }

    internal static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productName,
        string? productSku,
        Money unitPrice,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName.Trim(),
            ProductSku = productSku?.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }

    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        Quantity = quantity;
        SetUpdatedAt();
    }
}
