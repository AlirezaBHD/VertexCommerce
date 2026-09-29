using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace VertexCommerce.Shared.Services;

public class CloudinaryMediaService : IMediaService
{
    private static readonly string[] VideoExtensions = { ".mp4", ".webm", ".mov" };
    private static readonly HttpClient HttpClient = new();

    private readonly Cloudinary _cloudinary;

    public CloudinaryMediaService(IOptions<CloudinaryOptions> options)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.CloudName) ||
            string.IsNullOrWhiteSpace(settings.ApiKey) ||
            string.IsNullOrWhiteSpace(settings.ApiSecret))
        {
            throw new ArgumentException(
                "Cloudinary CloudName/ApiKey/ApiSecret must be configured when Media:Provider is set to Cloudinary.");
        }

        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);

        if (!string.IsNullOrWhiteSpace(settings.Proxy))
        {
            _cloudinary.Api.ApiProxy = settings.Proxy;
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var sanitizedFolder = Path.GetFileName(folder);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var isVideo = VideoExtensions.Contains(extension);

        RawUploadResult result;
        if (isVideo)
        {
            result = await _cloudinary.UploadAsync(new VideoUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = sanitizedFolder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
            }, cancellationToken: cancellationToken);
        }
        else
        {
            result = await _cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = sanitizedFolder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
            }, cancellationToken: cancellationToken);
        }

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var parsed = TryParseCloudinaryUrl(filePath);
        if (parsed is null)
        {
            // Not a Cloudinary URL (e.g. a legacy local path) — this provider doesn't own it.
            return false;
        }

        var (publicId, resourceType) = parsed.Value;

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = resourceType == "video" ? ResourceType.Video : ResourceType.Image,
        };

        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(filePath, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public bool FileExists(string filePath)
    {
        try
        {
            using var response = HttpClient.Send(new HttpRequestMessage(System.Net.Http.HttpMethod.Head, filePath));
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static (string PublicId, string ResourceType)? TryParseCloudinaryUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            !uri.Host.Contains("cloudinary.com", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var uploadIndex = Array.IndexOf(segments, "upload");
        if (uploadIndex < 1 || uploadIndex >= segments.Length - 1)
        {
            return null;
        }

        var resourceType = segments[uploadIndex - 1];
        var afterUpload = segments.Skip(uploadIndex + 1).ToArray();

        if (afterUpload.Length > 0 && IsVersionSegment(afterUpload[0]))
        {
            afterUpload = afterUpload.Skip(1).ToArray();
        }

        if (afterUpload.Length == 0)
        {
            return null;
        }

        var lastSegmentWithoutExtension = Path.GetFileNameWithoutExtension(afterUpload[^1]);
        var publicIdParts = afterUpload.Take(afterUpload.Length - 1).Append(lastSegmentWithoutExtension);
        var publicId = string.Join("/", publicIdParts);

        return (publicId, resourceType);
    }

    private static bool IsVersionSegment(string segment) =>
        segment.Length > 1 && segment[0] == 'v' && segment[1..].All(char.IsDigit);
}
