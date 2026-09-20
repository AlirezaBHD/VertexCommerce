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

public static class ShippingSettingsEndpoints
{
    public static void MapShippingSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var publicGroup = app.MapGroup("/api/shipping-settings")
            .WithTags("ShippingSettings");

        publicGroup.MapGet("/", Get)
            .WithName("GetShippingSettings")
            .Produces<ShippingSettingsResponse>(StatusCodes.Status200OK);

        var adminGroup = app.MapGroup("/api/shipping-settings")
            .WithTags("ShippingSettings")
            .RequireAuthorization(AppRoles.Admin);

        adminGroup.MapPut("/", Update)
            .WithName("UpdateShippingSettings")
            .Produces<ShippingSettingsResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Get(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetShippingSettingsQuery(), ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Update(
        [FromBody] UpdateShippingSettingsRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateShippingSettingsCommand(
            request.Cost,
            request.Currency,
            request.FreeShippingThreshold,
            request.Description,
            request.IsActive);

        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }
}

public sealed record UpdateShippingSettingsRequest(
    decimal Cost,
    string? Currency = "USD",
    decimal? FreeShippingThreshold = null,
    string? Description = null,
    bool IsActive = true);
