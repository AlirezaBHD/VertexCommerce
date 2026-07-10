using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Medias;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

internal sealed class MediaFileRepository : IMediaFileRepository
{
    private readonly CatalogDbContext _context;

    public MediaFileRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.MediaFiles.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task AddAsync(MediaFile mediaFile, CancellationToken ct = default)
        => await _context.MediaFiles.AddAsync(mediaFile, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
