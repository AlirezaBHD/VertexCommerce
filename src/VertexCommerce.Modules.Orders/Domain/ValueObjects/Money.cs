using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct Money
{
    public static StringFieldSchema CurrencySchema { get; } = new(maxLength: 3);

    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public static Money Create(decimal amount, string? currency = "USD")
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        return new Money
        {
            Amount = amount,
            Currency = StringFieldGuard.Apply(currency?.ToUpperInvariant(), CurrencySchema, nameof(Currency))
        };
    }

    public static Money Zero(string currency = "USD") => Create(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add money with different currencies.");
        }

        return new Money { Amount = Amount + other.Amount, Currency = Currency };
    }

    public Money Multiply(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        }

        return new Money { Amount = Amount * quantity, Currency = Currency };
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
