using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Orders.Endpoints;

public sealed record UpdateShippingSettingsRequest(
    decimal Cost,
    string? Currency = "USD",
    decimal? FreeShippingThreshold = null,
    string? Description = null,
    bool IsActive = true);
