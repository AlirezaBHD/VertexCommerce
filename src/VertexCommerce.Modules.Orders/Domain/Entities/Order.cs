using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.Events;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public string? CustomerEmail { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Address? BillingAddress { get; private set; }
    public Money SubTotal { get; private set; } = null!;
    public Money ShippingCost { get; private set; } = null!;
    public Money Tax { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = null!;
    public string? Notes { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order()
    {
    }

    public static Order Create(
        Guid customerId,
        string? customerEmail,
        Address shippingAddress,
        Address? billingAddress,
        string currency = "USD",
        string? notes = null)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            CustomerEmail = customerEmail,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            SubTotal = Money.Zero(currency),
            ShippingCost = Money.Zero(currency),
            Tax = Money.Zero(currency),
            TotalAmount = Money.Zero(currency),
            Notes = notes?.Trim()
        };

        return order;
    }

    public void AddItem(
        Guid productId,
        string productName,
        string? productSku,
        Money unitPrice,
        int quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Cannot add items to a non-pending order.");
        }

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = OrderItem.Create(Id, productId, productName, productSku, unitPrice, quantity);
            _items.Add(item);
        }

        RecalculateTotals();
        SetUpdatedAt();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Cannot remove items from a non-pending order.");
        }

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _items.Remove(item);
            RecalculateTotals();
            SetUpdatedAt();
        }
    }

    public void SetShippingCost(Money shippingCost)
    {
        ShippingCost = shippingCost;
        RecalculateTotals();
        SetUpdatedAt();
    }

    public void SetTax(Money tax)
    {
        Tax = tax;
        RecalculateTotals();
        SetUpdatedAt();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be confirmed.");
        }

        if (!_items.Any())
        {
            throw new InvalidOperationException("Cannot confirm an order without items.");
        }

        var oldStatus = Status;
        Status = OrderStatus.Confirmed;
        SetUpdatedAt();

        AddDomainEvent(new OrderCreatedEvent(
            Id,
            CustomerId,
            OrderNumber,
            TotalAmount.Amount,
            TotalAmount.Currency
        ));

        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, Status));
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed orders can be processed.");
        }

        ChangeStatus(OrderStatus.Processing);
    }

    public void Ship()
    {
        if (Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException("Only processing orders can be shipped.");
        }

        ChangeStatus(OrderStatus.Shipped);
        ShippedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Only shipped orders can be delivered.");
        }

        ChangeStatus(OrderStatus.Delivered);
        DeliveredAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            throw new InvalidOperationException($"Cannot cancel an order with status {Status}.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));
        }

        var oldStatus = Status;
        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason.Trim();
        SetUpdatedAt();

        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, Status));
        AddDomainEvent(new OrderCancelledEvent(Id, OrderNumber, reason));
    }

    public void MarkAsPaid()
    {
        PaymentStatus = PaymentStatus.Paid;
        SetUpdatedAt();
    }

    public void MarkPaymentFailed()
    {
        PaymentStatus = PaymentStatus.Failed;
        SetUpdatedAt();
    }

    public void Refund()
    {
        if (PaymentStatus != PaymentStatus.Paid)
        {
            throw new InvalidOperationException("Only paid orders can be refunded.");
        }

        PaymentStatus = PaymentStatus.Refunded;
        Status = OrderStatus.Refunded;
        SetUpdatedAt();
    }

    private void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        SetUpdatedAt();

        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, newStatus));
    }

    private void RecalculateTotals()
    {
        var currency = SubTotal.Currency;

        SubTotal = _items.Aggregate(
            Money.Zero(currency),
            (total, item) => total.Add(item.TotalPrice));

        TotalAmount = SubTotal.Add(ShippingCost).Add(Tax);
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
        return $"ORD-{timestamp}-{random}";
    }
}