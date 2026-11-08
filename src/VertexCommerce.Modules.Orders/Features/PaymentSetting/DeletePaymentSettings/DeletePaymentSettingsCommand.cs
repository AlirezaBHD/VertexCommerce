using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.DeletePaymentSettings;

public sealed record DeletePaymentSettingsCommand(Guid Id) : ICommand;
