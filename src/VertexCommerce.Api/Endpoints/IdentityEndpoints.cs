using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Modules.Identity.Features.GetProfile;
using VertexCommerce.Modules.Identity.Features.Login;
using VertexCommerce.Modules.Identity.Features.RefreshToken;
using VertexCommerce.Modules.Identity.Features.Register;

namespace VertexCommerce.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register", Register)
            .WithName("Register")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .Produces<AuthResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .Produces<AuthResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/profile", GetProfile)
            .WithName("GetProfile")
            .RequireAuthorization()
            .Produces<ProfileResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/auth/profile", result.Value)
            : Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Message,
            statusCode: HttpsExtension.GetStatusCode(result.Error.Code));
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Refresh(
        [FromBody] RefreshRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetProfile(
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = context.User.GetUserId();
        var query = new GetProfileQuery(userId);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }
}