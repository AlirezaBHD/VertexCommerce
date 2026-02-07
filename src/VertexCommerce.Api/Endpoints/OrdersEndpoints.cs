using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Modules.Orders.Features.CancelOrder;
using VertexCommerce.Modules.Orders.Features.CreateOrder;
using VertexCommerce.Modules.Orders.Features.GetOrderById;

namespace VertexCommerce.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetOrderById)
            .WithName("GetOrderById")
            .Produces<OrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/cancel", CancelOrder)
            .WithName("CancelOrder")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/orders/{result.Value}", result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        [FromBody] CancelOrderRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new CancelOrderCommand(id, request.Reason), ct);

        return result.IsSuccess
            ? Results.Ok()
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

public sealed record CancelOrderRequest(string Reason);