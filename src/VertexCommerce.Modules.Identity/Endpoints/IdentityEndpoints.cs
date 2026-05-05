using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Identity.Features.Commands.ChangePassword;
using VertexCommerce.Modules.Identity.Features.Commands.Login;
using VertexCommerce.Modules.Identity.Features.Commands.RefreshToken;
using VertexCommerce.Modules.Identity.Features.Commands.Registration;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.SendOpt;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.VerifyOtp;
using VertexCommerce.Modules.Identity.Features.Queries.GetProfile;

namespace VertexCommerce.Modules.Identity.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register/send-opt", SendOpt)
            .WithName("SendOpt")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);
        
        group.MapPost("/register/verify-opt", VerifyOpt)
            .WithName("VerifyOpt")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);
        
        group.MapPost("/register/complete", CompleteRegistration)
            .WithName("CompleteRegistration")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

        group.MapPatch("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

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

    private static async Task<IResult> SendOpt(
        [FromBody] SendOptRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new SendOptCommand(PhoneNumber: request.PhoneNumber);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> VerifyOpt(
        [FromBody] VerifyOptRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new VerifyOtpCommand(RegistrationToken: request.RegistrationToken, Otp: request.Opt);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new ChangePasswordCommand(
            CurrentPassword: request.CurrentPassword,
            NewPassword: request.NewPassword);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> CompleteRegistration(
        [FromBody] CompleteRegisterRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CompleteRegistrationCommand(
            RegistrationToken: request.RegistrationToken,
            Password: request.Password,
            FirstName: request.FirstName,
            LastName: request.LastName
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/auth/profile", result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.PhoneNumber, request.Password);
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
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetProfileQuery();
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }
}
