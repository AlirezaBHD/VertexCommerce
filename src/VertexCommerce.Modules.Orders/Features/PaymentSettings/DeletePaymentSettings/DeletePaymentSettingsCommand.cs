using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.DeletePaymentSettings;

public sealed record DeletePaymentSettingsCommand(Guid Id) : ICommand;
