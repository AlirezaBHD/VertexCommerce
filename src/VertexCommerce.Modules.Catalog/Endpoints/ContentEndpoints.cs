using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.ReorderBanners;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteBanner;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.SetActiveHero;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;
using VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;
using VertexCommerce.Modules.Catalog.Features.Content.Queries;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Modules.Catalog.Services;
using VertexCommerce.Shared.CQRS;
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

        bannerGroup.MapGet("/", GetAllBanners)
            .WithName("GetAllBanners")
            .Produces<IReadOnlyList<BannerDocument>>();

        bannerGroup.MapGet("/{id:guid}", GetBannerById)
            .WithName("GetBannerById")
            .Produces<BannerDocument>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        bannerGroup.MapDelete("/{id:guid}", DeleteBanner)
            .WithName("DeleteBanner")
            .Produces(StatusCodes.Status204NoContent);

        bannerGroup.MapPatch("/reorder", ReorderBanners)
            .WithName("ReorderBanners")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // ── Public Banners ─────────────────────────────────────────────────────

        var publicGroup = app.MapGroup("/api/banners").WithTags("Public");

        publicGroup.MapGet("/active", GetActiveBanners)
            .WithName("GetActiveBanners")
            .Produces<IReadOnlyList<BannerResponseDto>>();

        // ── Product & Category Lookup ─────────────────────────────────────────

        var lookupGroup = app.MapGroup("/api").WithTags("Lookup");

        lookupGroup.MapGet("/products/lookup", ProductLookup)
            .WithName("ProductLookup")
            .Produces<IReadOnlyList<ProductLookupItem>>();

        lookupGroup.MapGet("/categories/lookup", CategoryLookup)
            .WithName("CategoryLookup")
            .Produces<IReadOnlyList<CategoryLookupItem>>();

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
            request.Target,
            request.ImageMediaFileId,
            request.MobileImageMediaFileId,
            request.VideoMediaFileId,
            request.ImagePath,
            request.MobileImagePath,
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
            request.Target,
            request.MediaFileId,
            request.ImagePath,
            request.SortOrder,
            request.IsActive);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { Id = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetAllBanners(
        [FromServices] IContentRepository contentRepository,
        CancellationToken ct)
    {
        var banners = await contentRepository.GetAllBannersAsync(ct);
        return Results.Ok(banners);
    }

    private static async Task<IResult> GetBannerById(
        Guid id,
        [FromServices] IContentRepository contentRepository,
        CancellationToken ct)
    {
        var banner = await contentRepository.GetBannerByIdAsync(id, ct);
        if (banner is null)
            return Results.NotFound(new { message = $"Banner with id '{id}' not found." });
        return Results.Ok(banner);
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

    private static async Task<IResult> ReorderBanners(
        [FromBody] ReorderBannersRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new ReorderBannersCommand(request.Items);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    // ── Public Banner handlers ────────────────────────────────────────────────

    private static async Task<IResult> GetActiveBanners(
        [FromServices] IContentRepository contentRepository,
        [FromServices] ITargetResolver targetResolver,
        CancellationToken ct)
    {
        var banners = await contentRepository.GetActiveBannersAsync(ct);

        var result = banners.Select(b => new BannerResponseDto(
            b.Id,
            b.Title,
            b.Target,
            targetResolver.ResolveHref(b.Target, out var isExternal),
            isExternal,
            b.MediaFileId,
            b.ImagePath,
            b.SortOrder,
            b.IsActive,
            b.CreatedAt,
            b.UpdatedAt
        )).ToList();

        return Results.Ok(result.AsReadOnly());
    }

    // ── Lookup handlers ───────────────────────────────────────────────────────

    private static async Task<IResult> ProductLookup(
        [FromQuery] string? q,
        [FromQuery] int limit,
        [FromServices] IMongoDatabase database,
        CancellationToken ct)
    {
        var collection = database.GetCollection<ProductReadModel>("products");

        var filter = Builders<ProductReadModel>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.Trim();
            var regex = new MongoDB.Bson.BsonRegularExpression(search, "i");
            filter = Builders<ProductReadModel>.Filter.Or(
                Builders<ProductReadModel>.Filter.Regex(p => p.Name, regex),
                Builders<ProductReadModel>.Filter.Regex(p => p.Slug, regex),
                Builders<ProductReadModel>.Filter.Regex(p => p.SearchText, regex)
            );
        }

        limit = Math.Clamp(limit, 1, 50);

        var products = await collection.Find(filter)
            .Project(p => new ProductLookupItem(
                p.Id,
                p.Name,
                p.Slug,
                p.Media.Count > 0 ? p.Media[0].Path : null
            ))
            .Limit(limit)
            .ToListAsync(ct);

        return Results.Ok(products);
    }

    private static async Task<IResult> CategoryLookup(
        [FromQuery] string? q,
        [FromQuery] int limit,
        [FromServices] IMongoDatabase database,
        CancellationToken ct)
    {
        var collection = database.GetCollection<CategoryReadModel>("categories");

        var filter = Builders<CategoryReadModel>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.Trim();
            var regex = new MongoDB.Bson.BsonRegularExpression(search, "i");
            filter = Builders<CategoryReadModel>.Filter.Or(
                Builders<CategoryReadModel>.Filter.Regex(c => c.Name, regex),
                Builders<CategoryReadModel>.Filter.Regex(c => c.Slug, regex)
            );
        }

        limit = Math.Clamp(limit, 1, 50);

        var categories = await collection.Find(filter)
            .Project(c => new CategoryLookupItem(
                c.Id,
                c.Name,
                c.Slug,
                c.Path
            ))
            .Limit(limit)
            .ToListAsync(ct);

        return Results.Ok(categories);
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
