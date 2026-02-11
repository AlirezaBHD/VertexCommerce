using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Modules.Customers.Features.AddAddress;
using VertexCommerce.Modules.Customers.Features.GetCustomer;

namespace VertexCommerce.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers")
            .WithTags("Customers")
            .RequireAuthorization();

        group.MapGet("/me", GetMyProfile);
        group.MapPost("/me/addresses", AddAddress);

        return app;
    }

    private static async Task<IResult> GetMyProfile(
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = context.User.GetUserId();
        var query = new GetCustomerQuery(userId);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AddAddress(
        HttpContext context,
        [FromBody] AddAddressRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = context.User.GetUserId();

        var command = new AddAddressCommand(
            userId,
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode,
            request.Label
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/customers/me/addresses/{result.Value.Id}", result.Value)
            : result.Error.ToHttpResult();
    }
}
