using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public string CustomerPhoneNumber { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public string? ReceiptImagePath { get; private set; }
    public string? TransactionReference { get; private set; }
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
    public DateTime? ExpiresAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(
        Guid customerId,
        string customerPhoneNumber,
        Address shippingAddress,
        Address billingAddress,
        string currency = "USD",
        string? notes = null)
    {
        return CreateOrder(
            customerId: customerId,
            customerPhoneNumber: customerPhoneNumber,
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            status: OrderStatus.Pending,
            paymentStatus: PaymentStatus.Pending,
            currency: currency,
            notes: notes,
            expiresAt: DateTime.UtcNow.AddMinutes(10));
    }

    public static Order CreateManual(
        Guid customerId,
        string customerPhoneNumber,
        Address shippingAddress,
        Address billingAddress,
        string currency = "USD",
        string? notes = null)
    {
        return CreateOrder(
            customerId: customerId,
            customerPhoneNumber: customerPhoneNumber,
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            status: OrderStatus.Confirmed,
            paymentStatus: PaymentStatus.Paid,
            currency: currency,
            notes: notes,
            expiresAt: null);
    }

    private static Order CreateOrder(
        Guid customerId,
        string customerPhoneNumber,
        Address shippingAddress,
        Address billingAddress,
        OrderStatus status,
        PaymentStatus paymentStatus,
        string currency,
        string? notes,
        DateTime? expiresAt)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            CustomerPhoneNumber = customerPhoneNumber,
            Status = status,
            PaymentStatus = paymentStatus,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            SubTotal = Money.Zero(currency),
            ShippingCost = Money.Zero(currency),
            Tax = Money.Zero(currency),
            TotalAmount = Money.Zero(currency),
            ConfirmedAt = status == OrderStatus.Confirmed ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    public void AddItem(
        Guid productId,
        Guid variantId,
        string productName,
        string? productSku,
        Money unitPrice,
        int quantity)
    {
        ArgumentNullException.ThrowIfNull(unitPrice);

        var item = OrderItem.Create(
            orderId: Id,
            productId: productId,
            variantId: variantId,
            productName: productName,
            productSku: productSku,
            unitPrice: unitPrice,
            quantity: quantity);

        _items.Add(item);

        RecalculateTotals();
    }

    #region State Transitions

    public Result Confirm()
    {
        if (Status != OrderStatus.PaymentUnderReview)
            return Result.Failure(Error.Validation($"Cannot confirm order with status {Status}"));

        if (!_items.Any())
            return Result.Failure(Error.Validation("Cannot confirm order without items"));

        Status = OrderStatus.Confirmed;
        PaymentStatus = PaymentStatus.Paid;
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
    public Result<string> SubmitPaymentReceipt(string receiptImagePath)
    {
        if (Status != OrderStatus.AwaitingPayment && Status != OrderStatus.Pending)
            return Result.Failure<string>(Error.Validation($"Cannot submit payment for order with status {Status}"));
            
        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value)
            return Result.Failure<string>(Error.Validation("Payment time expired"));
            
        ReceiptImagePath = receiptImagePath;
        TransactionReference = GenerateTransactionReference();
        Status = OrderStatus.PaymentUnderReview;
        return Result.Success(TransactionReference);
    }
    
    public Result InitiatePayment()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(Error.Validation($"Cannot initiate payment for order with status {Status}"));

        if (!_items.Any())
            return Result.Failure(Error.Validation("Cannot initiate payment order without items"));

        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value)
            return Result.Failure(Error.Validation("Payment time expired"));

        Status = OrderStatus.AwaitingPayment;
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
        if (Status is not (OrderStatus.Pending or OrderStatus.AwaitingPayment or OrderStatus.Confirmed))
            return Result.Failure(Error.Validation($"Cannot cancel order with status {Status}"));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("Cancellation reason is required"));

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Expire()
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.AwaitingPayment))
            return Result.Failure(Error.Validation($"Cannot expire order with status {Status}"));

        Status = OrderStatus.Cancelled;
        CancellationReason = "Payment timeout expired";
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
    
    private static string GenerateTransactionReference()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}