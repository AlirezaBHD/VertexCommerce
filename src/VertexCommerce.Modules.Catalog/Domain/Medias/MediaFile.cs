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
}