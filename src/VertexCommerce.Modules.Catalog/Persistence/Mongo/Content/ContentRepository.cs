using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;

internal sealed class ContentRepository(IMongoDatabase database) : IContentRepository
{
    private readonly IMongoCollection<HeroContentDocument> _heroes =
        database.GetCollection<HeroContentDocument>("hero_contents");

    private readonly IMongoCollection<BannerDocument> _banners =
        database.GetCollection<BannerDocument>("banners");

    // ── Hero ─────────────────────────────────────────────────────────────────

    public async Task<HeroContentDocument?> GetActiveHeroAsync(CancellationToken ct = default)
        => await _heroes.Find(h => h.IsActive).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<HeroContentDocument>> GetAllHeroAsync(CancellationToken ct = default)
        => await _heroes.Find(FilterDefinition<HeroContentDocument>.Empty)
            .SortByDescending(h => h.UpdatedAt)
            .ToListAsync(ct);

    public async Task UpsertHeroAsync(HeroContentDocument hero, CancellationToken ct = default)
    {
        hero.UpdatedAt = DateTime.UtcNow;
        await _heroes.ReplaceOneAsync(
            h => h.Id == hero.Id,
            hero,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task SetActiveHeroAsync(Guid id, CancellationToken ct = default)
    {
        // deactivate all
        await _heroes.UpdateManyAsync(
            FilterDefinition<HeroContentDocument>.Empty,
            Builders<HeroContentDocument>.Update.Set(h => h.IsActive, false),
            cancellationToken: ct);

        // activate target
        await _heroes.UpdateOneAsync(
            h => h.Id == id,
            Builders<HeroContentDocument>.Update
                .Set(h => h.IsActive, true)
                .Set(h => h.UpdatedAt, DateTime.UtcNow),
            cancellationToken: ct);
    }

    public async Task DeleteHeroAsync(Guid id, CancellationToken ct = default)
        => await _heroes.DeleteOneAsync(h => h.Id == id, ct);

    // ── Banners ───────────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<BannerDocument>> GetActiveBannersAsync(CancellationToken ct = default)
        => await _banners.Find(b => b.IsActive)
            .SortBy(b => b.SortOrder)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<BannerDocument>> GetAllBannersAsync(CancellationToken ct = default)
        => await _banners.Find(FilterDefinition<BannerDocument>.Empty)
            .SortBy(b => b.SortOrder)
            .ToListAsync(ct);

    public async Task<BannerDocument?> GetBannerByIdAsync(Guid id, CancellationToken ct = default)
        => await _banners.Find(b => b.Id == id).FirstOrDefaultAsync(ct);

    public async Task UpsertBannerAsync(BannerDocument banner, CancellationToken ct = default)
    {
        banner.UpdatedAt = DateTime.UtcNow;
        await _banners.ReplaceOneAsync(
            b => b.Id == banner.Id,
            banner,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteBannerAsync(Guid id, CancellationToken ct = default)
        => await _banners.DeleteOneAsync(b => b.Id == id, ct);
}
