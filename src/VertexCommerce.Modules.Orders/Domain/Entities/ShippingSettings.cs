using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class ShippingSettings : Entity<Guid>
{
    public Money Cost { get; private set; }
    public Money FreeShippingThreshold { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private ShippingSettings() { }

    public static ShippingSettings Create(
        Money cost,
        Money? freeShippingThreshold = null,
        string? description = null,
        bool isActive = true)
    {
        var currency = cost.Currency;
        return new ShippingSettings
        {
            Id = Guid.NewGuid(),
            Cost = cost,
            FreeShippingThreshold = freeShippingThreshold is { Amount: > 0 }
                ? freeShippingThreshold.Value
                : Money.Zero(currency),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(
        Money cost,
        Money? freeShippingThreshold = null,
        string? description = null,
        bool isActive = true)
    {
        var currency = cost.Currency;
        Cost = cost;
        FreeShippingThreshold = freeShippingThreshold is { Amount: > 0 }
            ? freeShippingThreshold.Value
            : Money.Zero(currency);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = isActive;
        SetUpdatedAt();
    }

    public Money CalculateShippingCost(Money subtotal)
    {
        if (!IsActive)
        {
            return Money.Zero(subtotal.Currency);
        }

        if (FreeShippingThreshold.Amount > 0 && subtotal.Amount >= FreeShippingThreshold.Amount)
        {
            return Money.Zero(subtotal.Currency);
        }

        return Money.Create(Cost.Amount, subtotal.Currency);
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
