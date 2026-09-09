using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Domain.Errors;

public static class PaymentSettingsErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("PaymentSettings", id);

    public static readonly Error ActiveCannotDelete =
        Error.Validation("Cannot delete active payment settings.");

    public static readonly Error NoActiveSettings =
        Error.NotFound("PaymentSettings.Active", "No active payment settings found.");
}
