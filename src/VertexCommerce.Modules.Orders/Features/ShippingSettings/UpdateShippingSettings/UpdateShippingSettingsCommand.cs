using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings.UpdateShippingSettings;

public sealed record UpdateShippingSettingsCommand(
    decimal Cost,
    string? Currency = "USD",
    decimal? FreeShippingThreshold = null,
    string? Description = null,
    bool IsActive = true) : ICommand<ShippingSettingsResponse>;
