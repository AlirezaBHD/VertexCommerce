using DomainEntity = VertexCommerce.Modules.Orders.Domain.Entities.ShippingSettings;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings;

internal static class ShippingSettingsMapper
{
    public static ShippingSettingsResponse ToResponse(this DomainEntity s) => new(
        s.Id,
        s.Cost.Amount,
        s.Cost.Currency,
        s.FreeShippingThreshold.Amount > 0 ? s.FreeShippingThreshold.Amount : null,
        s.Description,
        s.IsActive,
        s.UpdatedAt ?? s.CreatedAt);

    public static ShippingSettingsResponse ToDefaultResponse(string currency = "USD") => new(
        null,
        0m,
        currency,
        null,
        null,
        true,
        null);
}
