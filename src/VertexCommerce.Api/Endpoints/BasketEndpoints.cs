using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Modules.Basket.Features.AddItem;
using VertexCommerce.Modules.Basket.Features.ClearBasket;
using VertexCommerce.Modules.Basket.Features.GetBasket;
using VertexCommerce.Modules.Basket.Features.RemoveItem;
using VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

namespace VertexCommerce.Api.Endpoints;

public static class BasketEndpoints
{
    public static IEndpointRouteBuilder MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/basket")
            .WithTags("Basket")
            .RequireAuthorization(); // All endpoints require auth

        group.MapGet("/", GetBasket)
            .WithName("GetBasket")
            .Produces<BasketResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/items", AddItem)
            .WithName("AddBasketItem")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/items", UpdateItemQuantity)
            .WithName("UpdateBasketItemQuantity")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/items/{productId:guid}", RemoveItem)
            .WithName("RemoveBasketItem")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/", ClearBasket)
            .WithName("ClearBasket")
            .Produces(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> GetBasket(
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var customerId = context.User.GetUserId();
        var result = await sender.Send(new GetBasketQuery(customerId), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AddItem(
        HttpContext context,
        [FromBody] AddItemRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var customerId = context.User.GetUserId();

        var command = new AddItemCommand(
            customerId,
            request.ProductId,
            request.Quantity
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { Message = "Item added to basket" })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateItemQuantity(
        HttpContext context,
        [FromBody] UpdateItemQuantityRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var customerId = context.User.GetUserId();

        var command = new UpdateItemQuantityCommand(
            customerId,
            request.ProductId,
            request.Quantity
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { Message = "Quantity updated" })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> RemoveItem(
        HttpContext context,
        Guid productId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var customerId = context.User.GetUserId();
        var result = await sender.Send(new RemoveItemCommand(customerId, productId), ct);

        return result.IsSuccess
            ? Results.Ok(new { Message = "Item removed" })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ClearBasket(
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var customerId = context.User.GetUserId();
        var result = await sender.Send(new ClearBasketCommand(customerId), ct);

        return result.IsSuccess
            ? Results.Ok(new { Message = "Basket cleared" })
            : result.Error.ToHttpResult();
    }
}