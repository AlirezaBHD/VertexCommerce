using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.SetActivePaymentSettings;

public sealed record SetActivePaymentSettingsCommand(Guid Id) : ICommand;
