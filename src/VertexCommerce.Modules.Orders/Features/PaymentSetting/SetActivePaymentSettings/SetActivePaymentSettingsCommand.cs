using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSetting.SetActivePaymentSettings;

public sealed record SetActivePaymentSettingsCommand(Guid Id) : ICommand;
