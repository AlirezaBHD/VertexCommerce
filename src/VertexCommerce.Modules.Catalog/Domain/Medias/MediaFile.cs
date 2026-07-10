using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Medias;

public class MediaFile : AggregateRoot<Guid>
{
    public string RelativePath { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public MediaFileStatus Status { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    private MediaFile()
    {
    }

    public static MediaFile Create(
        string relativePath,
        string originalFileName,
        string contentType,
        long sizeBytes)
    {
        return new MediaFile
        {
            Id = Guid.NewGuid(),
            RelativePath = relativePath,
            OriginalFileName = originalFileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            Status = MediaFileStatus.Pending,
        };
    }

    public void Confirm()
    {
        if (Status is MediaFileStatus.Confirmed)
            return;

        Status = MediaFileStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void MarkDeleted()
    {
        Status = MediaFileStatus.Deleted;
    }
}