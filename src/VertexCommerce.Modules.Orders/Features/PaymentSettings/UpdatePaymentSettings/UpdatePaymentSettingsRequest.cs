namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.UpdatePaymentSettings;

public sealed record UpdatePaymentSettingsRequest(
    string BankName,
    string AccountHolderName,
    string CardNumber,
    string? ShabaNumber,
    string? AccountNumber,
    string? Description);