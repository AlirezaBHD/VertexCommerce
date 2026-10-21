namespace VertexCommerce.Modules.Orders.Features.PaymentSettings;

public sealed record PaymentSettingsResponse(
    Guid Id,
    string BankName,
    string AccountHolderName,
    string CardNumber,
    string? ShabaNumber,
    string? AccountNumber,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);