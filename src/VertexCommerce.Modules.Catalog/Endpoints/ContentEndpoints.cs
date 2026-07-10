using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteBanner;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.SetActiveHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Catalog.Endpoints;

public static class ContentEndpoints
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder app)
    {
        // ── Hero ─────────────────────────────────────────────────────────────

        var heroGroup = app.MapGroup("/api/content/hero").WithTags("Content");

        heroGroup.MapPost("/", CreateOrUpdateHero)
            .WithName("CreateOrUpdateHero")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        heroGroup.MapPost("/{id:guid}/set-active", SetActiveHero)
            .WithName("SetActiveHero")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        heroGroup.MapDelete("/{id:guid}", DeleteHero)
            .WithName("DeleteHero")
            .Produces(StatusCodes.Status204NoContent);

        // ── Banners ───────────────────────────────────────────────────────────

        var bannerGroup = app.MapGroup("/api/content/banners").WithTags("Content");

        bannerGroup.MapPost("/", CreateOrUpdateBanner)
            .WithName("CreateOrUpdateBanner")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        bannerGroup.MapDelete("/{id:guid}", DeleteBanner)
            .WithName("DeleteBanner")
            .Produces(StatusCodes.Status204NoContent);

        // ── About ──────────────────────────────────────────────────────────────

        var aboutGroup = app.MapGroup("/api/content/about").WithTags("Content");

        aboutGroup.MapPost("/", UpdateAbout)
            .WithName("UpdateAbout")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // ── Contact ────────────────────────────────────────────────────────────

        var contactGroup = app.MapGroup("/api/content/contact").WithTags("Content");

        contactGroup.MapPost("/", UpdateContact)
            .WithName("UpdateContact")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    // ── Hero handlers ─────────────────────────────────────────────────────────

    private static async Task<IResult> CreateOrUpdateHero(
        [FromBody] CreateOrUpdateHeroRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CreateOrUpdateHeroCommand(
            request.Id,
            request.Title,
            request.RedirectPath,
            request.ImageMediaFileId,
            request.VideoMediaFileId,
            request.ImagePath,
            request.VideoPath,
            request.IsActive);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { Id = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> SetActiveHero(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new SetActiveHeroCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> DeleteHero(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteHeroCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    // ── Banner handlers ───────────────────────────────────────────────────────

    private static async Task<IResult> CreateOrUpdateBanner(
        [FromBody] CreateOrUpdateBannerRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CreateOrUpdateBannerCommand(
            request.Id,
            request.Title,
            request.RedirectPath,
            request.MediaFileId,
            request.ImagePath,
            request.SortOrder,
            request.IsActive);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { Id = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> DeleteBanner(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteBannerCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    // ── About handler ─────────────────────────────────────────────────────────

    private static async Task<IResult> UpdateAbout(
        [FromBody] UpdateAboutRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateAboutCommand(
            request.Title,
            request.Subtitle,
            request.Description,
            request.Mission,
            request.Vision,
            request.Values,
            request.Stats,
            request.Team);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    // ── Contact handler ───────────────────────────────────────────────────────

    private static async Task<IResult> UpdateContact(
        [FromBody] UpdateContactRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateContactCommand(
            request.Title,
            request.Subtitle,
            request.Description,
            request.Email,
            request.Phone,
            request.Address,
            request.WorkingHours,
            request.MapEmbedUrl,
            request.SocialLinks);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
}
