using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.CreatePaymentSettings;

public sealed record CreatePaymentSettingsCommand(
    string BankName,
    string AccountHolderName,
    string CardNumber,
    string? ShabaNumber,
    string? AccountNumber,
    string? Description
) : ICommand<PaymentSettingsResponse>;
