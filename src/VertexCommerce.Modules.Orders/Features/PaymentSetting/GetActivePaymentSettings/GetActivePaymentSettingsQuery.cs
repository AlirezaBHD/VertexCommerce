using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.GetActivePaymentSettings;

public sealed record GetActivePaymentSettingsQuery : IQuery<PaymentSettingsResponse>;
