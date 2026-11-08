using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.GetAllPaymentSettings;

public sealed record GetAllPaymentSettingsQuery : IQuery<IReadOnlyList<PaymentSettingsResponse>>;
