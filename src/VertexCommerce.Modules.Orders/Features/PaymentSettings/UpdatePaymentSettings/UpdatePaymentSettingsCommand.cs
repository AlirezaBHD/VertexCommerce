using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.UpdatePaymentSettings;

public sealed record UpdatePaymentSettingsCommand(
    Guid Id,
    string BankName,
    string AccountHolderName,
    string CardNumber,
    string? ShabaNumber,
    string? AccountNumber,
    string? Description
) : ICommand<PaymentSettingsResponse>;
