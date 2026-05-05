using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Orders.Features.CancelOrder;
using VertexCommerce.Modules.Orders.Features.ConfirmOrder;
using VertexCommerce.Modules.Orders.Features.DeliverOrder;
using VertexCommerce.Modules.Orders.Features.GetAllOrders;
using VertexCommerce.Modules.Orders.Features.GetMyOrderById;
using VertexCommerce.Modules.Orders.Features.GetMyOrders;
using VertexCommerce.Modules.Orders.Features.GetOrderById;
using VertexCommerce.Modules.Orders.Features.InitiatePayment;
using VertexCommerce.Modules.Orders.Features.ProcessOrder;
using VertexCommerce.Modules.Orders.Features.ShipOrder;
using VertexCommerce.Modules.Orders.Features.SubmitPaymentReceipt;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Orders.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        var customerGroup = group.MapGroup("/")
            .RequireAuthorization();

        customerGroup.MapGet("/my", GetMyOrders);
        customerGroup.MapGet("/my/{id:guid}", GetOrder);
        customerGroup.MapPost("/{id:guid}/payment/initiate", InitiatePayment);
        customerGroup.MapPost("/{id:guid}/payment/receipt", SubmitPaymentReceipt).DisableAntiforgery();
        customerGroup.MapPost("/{id:guid}/cancel", CancelOrder);

        var adminGroup = group.MapGroup("/");
            // .RequireAuthorization("Admin");

        adminGroup.MapGet("/", GetAllOrders);
        adminGroup.MapGet("/{id:guid}", GetOrderById);
        adminGroup.MapPost("/{id:guid}/confirm", ConfirmOrder);
        adminGroup.MapPost("/{id:guid}/process", ProcessOrder);
        adminGroup.MapPost("/{id:guid}/ship", ShipOrder);
        adminGroup.MapPost("/{id:guid}/deliver", DeliverOrder);

        return app;
    }


    private static async Task<IResult> InitiatePayment(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new InitiatePaymentCommand(OrderId: id);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
    
    private static async Task<IResult> GetAllOrders(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetAllOrdersQuery()
        {
            Page = page,
            PageSize = pageSize
        };
        
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> SubmitPaymentReceipt(
        [FromRoute] Guid id,
        IFormFile file,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var command = new SubmitPaymentReceiptCommand(OrderId: id, ReceiptFile: stream);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetMyOrders(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetMyOrdersQuery()
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private async static Task<IResult> GetOrder(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetMyOrderByIdQuery(OrderId: id);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
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
    private static async Task<IResult> GetOrderById(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
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
