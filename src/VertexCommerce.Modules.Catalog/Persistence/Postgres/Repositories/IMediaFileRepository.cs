using VertexCommerce.Modules.Catalog.Domain.Medias;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

public interface IMediaFileRepository
{
    Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(MediaFile mediaFile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
