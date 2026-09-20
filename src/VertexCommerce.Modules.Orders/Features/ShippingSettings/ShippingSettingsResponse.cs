namespace VertexCommerce.Modules.Orders.Features.ShippingSettings;

public sealed record ShippingSettingsResponse(
    Guid? Id,
    decimal Cost,
    string Currency,
    decimal? FreeShippingThreshold,
    string? Description,
    bool IsActive,
    DateTime? UpdatedAt);
