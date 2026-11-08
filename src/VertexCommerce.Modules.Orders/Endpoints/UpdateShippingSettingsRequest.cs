using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Orders.Features.ShippingSettings;
using VertexCommerce.Modules.Orders.Features.ShippingSettings.GetShippingSettings;
using VertexCommerce.Modules.Orders.Features.ShippingSettings.UpdateShippingSettings;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Orders.Endpoints;

public sealed record UpdateShippingSettingsRequest(
    decimal Cost,
    string? Currency = "USD",
    decimal? FreeShippingThreshold = null,
    string? Description = null,
    bool IsActive = true);
