using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Modules.Basket.Features.AddItem;
using VertexCommerce.Modules.Basket.Features.ClearBasket;
using VertexCommerce.Modules.Basket.Features.GetBasket;
using VertexCommerce.Modules.Basket.Features.RemoveItem;
using VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

namespace VertexCommerce.Api.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/basket")
            .WithTags("Basket");

        group.MapGet("/{customerId:guid}", GetBasket)
            .WithName("GetBasket")
            .Produces<BasketResponse>();

        group.MapPost("/items", AddItem)
            .WithName("AddBasketItem")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPut("/items", UpdateItemQuantity)
            .WithName("UpdateBasketItemQuantity")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{customerId:guid}/items/{productId:guid}", RemoveItem)
            .WithName("RemoveBasketItem")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{customerId:guid}", ClearBasket)
            .WithName("ClearBasket")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetBasket(
        Guid customerId,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetBasketQuery(customerId), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> AddItem(
        [FromBody] AddItemCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> UpdateItemQuantity(
        [FromBody] UpdateItemQuantityCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: GetStatusCode(result.Error.Code));
    }

    private static async Task<IResult> RemoveItem(
        Guid customerId,
        Guid productId,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new RemoveItemCommand(customerId, productId), ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ClearBasket(
        Guid customerId,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ClearBasketCommand(customerId), ct);

        return result.IsSuccess
            ? Results.Ok()
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
    }

    private static int GetStatusCode(string errorCode) => errorCode switch
    {
        _ when errorCode.Contains("NotFound") => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status400BadRequest
    };
}