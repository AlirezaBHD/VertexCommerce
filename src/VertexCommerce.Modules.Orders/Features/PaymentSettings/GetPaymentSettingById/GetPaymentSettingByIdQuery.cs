using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettingById;

public sealed record GetPaymentSettingByIdQuery(Guid Id) : IQuery<PaymentSettingsResponse>;