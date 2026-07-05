using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettings;

public sealed record GetAllPaymentSettingsQuery : IQuery<IReadOnlyList<PaymentSettingsResponse>>;

public sealed record GetActivePaymentSettingsQuery : IQuery<PaymentSettingsResponse>;
