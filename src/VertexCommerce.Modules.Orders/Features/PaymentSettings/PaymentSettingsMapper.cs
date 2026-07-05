using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using DomainEntity = VertexCommerce.Modules.Orders.Domain.Entities.PaymentSettings;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings;

internal static class PaymentSettingsMapper
{
    public static PaymentSettingsResponse ToResponse(this DomainEntity s) => new(
        s.Id, s.BankName, s.AccountHolderName, s.CardNumber,
        s.ShabaNumber, s.AccountNumber, s.Description,
        s.IsActive, s.CreatedAt, s.UpdatedAt);
}
