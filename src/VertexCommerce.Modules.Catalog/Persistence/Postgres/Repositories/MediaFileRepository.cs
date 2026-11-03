using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Medias;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

internal sealed class MediaFileRepository(CatalogDbContext context) : IMediaFileRepository
{
    public async Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.MediaFiles.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task AddAsync(MediaFile mediaFile, CancellationToken ct = default)
        => await context.MediaFiles.AddAsync(mediaFile, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
