using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Endpoints;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(this IEndpointRouteBuilder app)
    {
        // ── Hero ─────────────────────────────────────────────────────────────

        var heroGroup = app.MapGroup("/api/content/hero").WithTags("Content");

        heroGroup.MapGet("/", GetAllHero)
            .WithName("GetAllHero")
            .Produces<IReadOnlyList<HeroContentDocument>>();

        heroGroup.MapPost("/", CreateOrUpdateHero)
            .WithName("CreateOrUpdateHero")
            .Produces<HeroContentDocument>(StatusCodes.Status200OK);

        heroGroup.MapPost("/{id:guid}/set-active", SetActiveHero)
            .WithName("SetActiveHero")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        heroGroup.MapDelete("/{id:guid}", DeleteHero)
            .WithName("DeleteHero")
            .Produces(StatusCodes.Status204NoContent);

        // ── Banners ───────────────────────────────────────────────────────────

        var bannerGroup = app.MapGroup("/api/content/banners").WithTags("Content");

        bannerGroup.MapGet("/", GetAllBanners)
            .WithName("GetAllBanners")
            .Produces<IReadOnlyList<BannerDocument>>();

        bannerGroup.MapPost("/", CreateOrUpdateBanner)
            .WithName("CreateOrUpdateBanner")
            .Produces<BannerDocument>(StatusCodes.Status200OK);

        bannerGroup.MapDelete("/{id:guid}", DeleteBanner)
            .WithName("DeleteBanner")
            .Produces(StatusCodes.Status204NoContent);
    }

    // ── Hero handlers ─────────────────────────────────────────────────────────

    private static async Task<IResult> GetAllHero(
        IContentRepository repo, CancellationToken ct)
    {
        var items = await repo.GetAllHeroAsync(ct);
        return Results.Ok(items);
    }

    private static async Task<IResult> CreateOrUpdateHero(
        [FromBody] HeroUpsertRequest request,
        IContentRepository repo, CancellationToken ct)
    {
        var doc = new HeroContentDocument
        {
            Id = request.Id ?? Guid.NewGuid(),
            Title = request.Title,
            RedirectPath = request.RedirectPath,
            VideoPath = request.VideoPath,
            ImagePath = request.ImagePath,
            IsActive = request.IsActive,
        };
        await repo.UpsertHeroAsync(doc, ct);
        return Results.Ok(doc);
    }

    private static async Task<IResult> SetActiveHero(
        Guid id, IContentRepository repo, CancellationToken ct)
    {
        await repo.SetActiveHeroAsync(id, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteHero(
        Guid id, IContentRepository repo, CancellationToken ct)
    {
        await repo.DeleteHeroAsync(id, ct);
        return Results.NoContent();
    }

    // ── Banner handlers ───────────────────────────────────────────────────────

    private static async Task<IResult> GetAllBanners(
        IContentRepository repo, CancellationToken ct)
    {
        var items = await repo.GetAllBannersAsync(ct);
        return Results.Ok(items);
    }

    private static async Task<IResult> CreateOrUpdateBanner(
        [FromBody] BannerUpsertRequest request,
        IContentRepository repo, CancellationToken ct)
    {
        var doc = new BannerDocument
        {
            Id = request.Id ?? Guid.NewGuid(),
            Title = request.Title,
            RedirectPath = request.RedirectPath,
            ImagePath = request.ImagePath,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            CreatedAt = request.Id is null ? DateTime.UtcNow : default,
        };
        await repo.UpsertBannerAsync(doc, ct);
        return Results.Ok(doc);
    }

    private static async Task<IResult> DeleteBanner(
        Guid id, IContentRepository repo, CancellationToken ct)
    {
        await repo.DeleteBannerAsync(id, ct);
        return Results.NoContent();
    }
}

// ── Request DTOs ──────────────────────────────────────────────────────────────

public sealed record HeroUpsertRequest(
    Guid? Id,
    string Title,
    string RedirectPath,
    string? VideoPath,
    string? ImagePath,
    bool IsActive = false);

public sealed record BannerUpsertRequest(
    Guid? Id,
    string Title,
    string RedirectPath,
    string ImagePath,
    int SortOrder = 0,
    bool IsActive = true);
