using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Basket.Features.AddItem;
using VertexCommerce.Modules.Basket.Features.ClearBasket;
using VertexCommerce.Modules.Basket.Features.RemoveItem;
using VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Basket.Endpoints;

public static class BasketEndpoints
{
    public static IEndpointRouteBuilder MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/basket")
            .WithTags("Basket")
            .RequireAuthorization();

        group.MapPost("/items", AddItem)
            .WithName("AddBasketItem")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/items/{id:guid}", UpdateItemQuantity)
            .WithName("UpdateBasketItemQuantity")
            .Produces(StatusCodes.Status204NoContent)
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

    private static async Task<IResult> AddItem(
        [FromBody] AddItemRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AddItemCommand(
            VariantId: request.VariantId,
            ProductId: request.ProductId,
            Quantity: request.Quantity
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateItemQuantity(
        Guid id,
        [FromBody] UpdateItemQuantityRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateItemQuantityCommand(
            ProductId: id,
            VariantId: request.VariantId,
            Quantity: request.Quantity
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
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
