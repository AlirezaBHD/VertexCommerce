using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Orders.Features.Checkout;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Orders.Endpoints;

public static class CheckoutEndpoints
{
    public static void MapCheckoutEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checkout")
            .WithTags("Checkout")
            .RequireAuthorization();

        group.MapPost("/", Checkout)
            .WithName("Checkout")
            .Produces<CheckoutResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Checkout(
        [FromBody] CheckoutCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/orders/{result.Value.OrderId}", result.Value)
            : result.Error.ToHttpResult();
    }
}
