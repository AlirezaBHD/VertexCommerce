using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public string CustomerEmail { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }

    public Money SubTotal { get; private set; } = default!;
    public Money ShippingCost { get; private set; } = default!;
    public Money Tax { get; private set; } = default!;
    public Money TotalAmount { get; private set; } = default!;

    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;

    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? TrackingNumber { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ProcessingAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(
        Guid customerId,
        string customerEmail,
        Address shippingAddress,
        Address billingAddress,
        string currency = "USD",
        string? notes = null)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            CustomerEmail = customerEmail,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            Notes = notes,
            SubTotal = Money.Zero(currency),
            ShippingCost = Money.Zero(currency),
            Tax = Money.Zero(currency),
            TotalAmount = Money.Zero(currency),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(Guid productId, string productName, string productSku, Money unitPrice, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            // existingItem.IncreaseQuantity(quantity);
            //TODO
        }
        else
        {
            var item = OrderItem.Create(Id, productId, productName, productSku, unitPrice, quantity);
            _items.Add(item);
        }

        RecalculateTotals();
    }

    #region State Transitions

    public Result Confirm()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(Error.Validation($"Cannot confirm order with status {Status}"));

        if (!_items.Any())
            return Result.Failure(Error.Validation("Cannot confirm order without items"));

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            return Result.Failure(Error.Validation($"Cannot process order with status {Status}"));

        Status = OrderStatus.Processing;
        ProcessingAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            return Result.Failure(Error.Validation($"Cannot ship order with status {Status}"));

        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result.Failure(Error.Validation("Tracking number is required"));

        Status = OrderStatus.Shipped;
        TrackingNumber = trackingNumber;
        ShippedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure(Error.Validation($"Cannot deliver order with status {Status}"));

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            return Result.Failure(Error.Validation($"Cannot cancel order with status {Status}"));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("Cancellation reason is required"));

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    #endregion

    #region Payment

    public Result MarkAsPaid()
    {
        if (PaymentStatus == PaymentStatus.Paid)
            return Result.Failure(Error.Validation("Order is already paid"));

        PaymentStatus = PaymentStatus.Paid;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsRefunded()
    {
        if (PaymentStatus != PaymentStatus.Paid)
            return Result.Failure(Error.Validation("Can only refund paid orders"));

        PaymentStatus = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    #endregion

    public void SetShippingCost(Money shippingCost)
    {
        ShippingCost = shippingCost;
        RecalculateTotals();
    }

    public void SetTax(Money tax)
    {
        Tax = tax;
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        var currency = _items.FirstOrDefault()?.UnitPrice.Currency ?? "USD";

        SubTotal = Money.Create(
            _items.Sum(i => i.TotalPrice.Amount),
            currency
        );

        TotalAmount = Money.Create(
            SubTotal.Amount + ShippingCost.Amount + Tax.Amount,
            currency
        );

        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}