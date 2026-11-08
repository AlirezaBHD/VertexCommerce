using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShippingSetting.GetShippingSettings;

public sealed record GetShippingSettingsQuery : IQuery<ShippingSettingsResponse>;
