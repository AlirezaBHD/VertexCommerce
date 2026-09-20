using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings.GetShippingSettings;

public sealed record GetShippingSettingsQuery : IQuery<ShippingSettingsResponse>;
