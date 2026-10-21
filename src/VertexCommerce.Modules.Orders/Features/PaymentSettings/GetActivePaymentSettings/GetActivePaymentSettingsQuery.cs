using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetActivePaymentSettings;

public sealed record GetActivePaymentSettingsQuery : IQuery<PaymentSettingsResponse>;
