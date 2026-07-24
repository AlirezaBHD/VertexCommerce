using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Entities;

public sealed class PaymentSettings : Entity<Guid>
{
    public string BankName { get; private set; } = default!;
    public string AccountHolderName { get; private set; } = default!;
    public string CardNumber { get; private set; } = default!;
    public string? ShabaNumber { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private PaymentSettings() { }

    public static PaymentSettings Create(
        string bankName,
        string accountHolderName,
        string cardNumber,
        string? shabaNumber,
        string? accountNumber,
        string? description)
    {
        return new PaymentSettings
        {
            Id = Guid.NewGuid(),
            BankName = bankName.Trim(),
            AccountHolderName = accountHolderName.Trim(),
            CardNumber = cardNumber.Trim(),
            ShabaNumber = string.IsNullOrWhiteSpace(shabaNumber) ? null : shabaNumber.Trim(),
            AccountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(
        string bankName,
        string accountHolderName,
        string cardNumber,
        string? shabaNumber,
        string? accountNumber,
        string? description)
    {
        BankName = bankName.Trim();
        AccountHolderName = accountHolderName.Trim();
        CardNumber = cardNumber.Trim();
        ShabaNumber = string.IsNullOrWhiteSpace(shabaNumber) ? null : shabaNumber.Trim();
        AccountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
