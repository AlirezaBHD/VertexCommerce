using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetAllPaymentSettings;

public sealed record GetAllPaymentSettingsQuery : IQuery<IReadOnlyList<PaymentSettingsResponse>>;
