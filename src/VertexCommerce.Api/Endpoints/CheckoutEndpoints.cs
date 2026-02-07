using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Modules.Orders.Features.Checkout;

namespace VertexCommerce.Api.Endpoints;

public static class CheckoutEndpoints
{
    public static void MapCheckoutEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checkout")
            .WithTags("Checkout");

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
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: GetStatusCode(result.Error.Code));
    }

    private static int GetStatusCode(string errorCode) => errorCode switch
    {
        _ when errorCode.Contains("NotFound") => StatusCodes.Status404NotFound,
        _ when errorCode.Contains("Validation") => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
}
