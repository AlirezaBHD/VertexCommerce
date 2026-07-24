using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.DeletePaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettingById;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.SetActivePaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.UpdatePaymentSettings;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Orders.Endpoints;

public static class PaymentSettingsEndpoints
{
    public static void MapPaymentSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var publicGroup = app.MapGroup("/api/payment-settings")
            .WithTags("PaymentSettings");

        publicGroup.MapGet("/active", GetActive)
            .WithName("GetActivePaymentSettings")
            .Produces<PaymentSettingsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        var adminGroup = app.MapGroup("/api/payment-settings")
            .WithTags("PaymentSettings");
            // .RequireAuthorization("Admin");

        adminGroup.MapGet("/", GetAll)
            .WithName("GetAllPaymentSettings")
            .Produces<IReadOnlyList<PaymentSettingsResponse>>(StatusCodes.Status200OK);

        adminGroup.MapGet("/{id:guid}", GetById)
            .WithName("GetPaymentSettingsWithId")
            .Produces<PaymentSettingsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        
        adminGroup.MapPost("/", Create)
            .WithName("CreatePaymentSettings")
            .Produces<PaymentSettingsResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        adminGroup.MapPut("/{id:guid}", Update)
            .WithName("UpdatePaymentSettings")
            .Produces<PaymentSettingsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        adminGroup.MapPost("/{id:guid}/set-active", SetActive)
            .WithName("SetActivePaymentSettings")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        adminGroup.MapDelete("/{id:guid}", Delete)
            .WithName("DeletePaymentSettings")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
    
    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentSettingByIdQuery(id), ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetActive(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetActivePaymentSettingsQuery(), ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetAll(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAllPaymentSettingsQuery(), ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Create(
        [FromBody] CreatePaymentSettingsCommand command,
        ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/payment-settings/{result.Value.Id}", result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Update(
        Guid id,
        [FromBody] UpdatePaymentSettingsRequest request,
        ISender sender, CancellationToken ct)
    {
        var command = new UpdatePaymentSettingsCommand(
            id, request.BankName, request.AccountHolderName, request.CardNumber,
            request.ShabaNumber, request.AccountNumber, request.Description);
        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> SetActive(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new SetActivePaymentSettingsCommand(id), ct);
        return result.IsSuccess ? Results.NoContent() : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Delete(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeletePaymentSettingsCommand(id), ct);
        return result.IsSuccess ? Results.NoContent() : result.Error.ToHttpResult();
    }
}

public sealed record UpdatePaymentSettingsRequest(
    string BankName,
    string AccountHolderName,
    string CardNumber,
    string? ShabaNumber,
    string? AccountNumber,
    string? Description);
