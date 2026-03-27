using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Shared.Services;

namespace VertexCommerce.Api.Endpoints;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/media")
            .WithTags("Media");
            // .RequireAuthorization();

        group.MapPost("/upload", UploadMedia)
            .DisableAntiforgery()
            .WithName("UploadMedia")
            .WithSummary("Upload a single media file")
            .Produces<MediaUploadResponse>(200)
            .Produces(400);

        group.MapDelete("/{*path}", DeleteMedia)
            .WithName("DeleteMedia")
            .WithSummary("Delete a media file")
            .Produces(204)
            .Produces(404);
    }
    
    private static async Task<IResult> UploadMedia(
        IFormFile file,
        [FromQuery] string? folder,
        IMediaService mediaService,
        CancellationToken ct)
    {
        var validation = ValidateFile(file);
        if (validation is not null)
            return validation;

        await using var stream = file.OpenReadStream();
        var path = await mediaService.SaveFileAsync(stream, file.FileName, folder ?? "general", ct);

        return Results.Ok(new MediaUploadResponse(path, file.FileName, file.ContentType, file.Length));
    }


    private static async Task<IResult> DeleteMedia(
        string path,
        IMediaService mediaService,
        CancellationToken ct)
    {
        var deleted = await mediaService.DeleteFileAsync(path, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static IResult? ValidateFile(IFormFile file)
    {
        const long maxSize = 10 * 1024 * 1024; // 10MB
        
        var allowedTypes = new[]
        {
            "image/jpeg", "image/png", "image/gif", "image/webp",
            "video/mp4", "video/webm", "video/quicktime"
        };

        if (file.Length == 0)
            return Results.BadRequest(new { Error = "Empty file" });

        if (file.Length > maxSize)
            return Results.BadRequest(new { Error = "File too large. Maximum 10MB" });

        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return Results.BadRequest(new { Error = $"Invalid file type: {file.ContentType}" });

        return null;
    }
}

public sealed record MediaUploadResponse(
    string Path,
    string FileName,
    string ContentType,
    long Size
);
