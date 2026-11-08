using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.GetPaymentSettingById;

public sealed record GetPaymentSettingByIdQuery(Guid Id) : IQuery<PaymentSettingsResponse>;