using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Modules.Orders.Features.CancelOrder;
using VertexCommerce.Modules.Orders.Features.ConfirmOrder;
using VertexCommerce.Modules.Orders.Features.DeliverOrder;
using VertexCommerce.Modules.Orders.Features.ProcessOrder;
using VertexCommerce.Modules.Orders.Features.ShipOrder;

namespace VertexCommerce.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        var customerGroup = group.MapGroup("/")
            .RequireAuthorization();

        customerGroup.MapGet("/my", GetMyOrders);
        customerGroup.MapGet("/{id:guid}", GetOrder);
        customerGroup.MapPost("/{id:guid}/cancel", CancelOrder);

        var adminGroup = group.MapGroup("/")
            .RequireAuthorization("Admin");

        adminGroup.MapPost("/{id:guid}/confirm", ConfirmOrder);
        adminGroup.MapPost("/{id:guid}/process", ProcessOrder);
        adminGroup.MapPost("/{id:guid}/ship", ShipOrder);
        adminGroup.MapPost("/{id:guid}/deliver", DeliverOrder);

        return app;
    }

    private static Task<IResult> GetMyOrders(
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = context.User.GetUserId();
        // TODO: Implement GetOrdersByCustomerQuery
        return Task.FromResult(Results.Ok(new { Message = "Not implemented yet", UserId = userId }));
    }

    private static Task<IResult> GetOrder(
        Guid id,
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        // TODO: Implement GetOrderByIdQuery with authorization check
        return Task.FromResult(Results.Ok(new { Message = "Not implemented yet", OrderId = id }));
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        [FromBody] CancelOrderRequest request,
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CancelOrderCommand(id, request.Reason);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ConfirmOrder(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ConfirmOrderCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ProcessOrder(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ProcessOrderCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ShipOrder(
        Guid id,
        [FromBody] ShipOrderRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ShipOrderCommand(id, request.TrackingNumber), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> DeliverOrder(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeliverOrderCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
}

