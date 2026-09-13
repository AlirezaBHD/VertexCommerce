namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new VertexCommerce.Shared.Exceptions.DomainValidationException("Catalog.Money", "Amount cannot be negative.");
            
        return new Money(amount, currency);
    }
}
