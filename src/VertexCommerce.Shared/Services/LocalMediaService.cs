using Microsoft.Extensions.Options;

namespace VertexCommerce.Shared.Services;

public class LocalMediaService : IMediaService
{
    private readonly string _basePath;
    private readonly string _uploadPath;

    public LocalMediaService(IOptions<MediaOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.RootPath))
        {
            throw new ArgumentException("RootPath is not configured in MediaOptions.");
        }

        _uploadPath = "uploads";
        _basePath = Path.Combine(options.Value.RootPath, _uploadPath);
        
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var sanitizedFolder = Path.GetFileName(folder); 
        
        var folderPath = Path.Combine(_basePath, sanitizedFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
        var filePath = Path.Combine(folderPath, uniqueFileName);

        await using var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return Path.Combine(_uploadPath,sanitizedFolder, uniqueFileName).Replace("\\", "/");
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetValidatedFullPath(filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetValidatedFullPath(filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Media file not found.", filePath);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        return Task.FromResult(stream);
    }

    public bool FileExists(string filePath)
    {
        var fullPath = GetValidatedFullPath(filePath);
        return File.Exists(fullPath);
    }

    private string GetValidatedFullPath(string filePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_basePath, filePath));
        
        if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid file path.");
        }
        
        return fullPath;
    }
}
