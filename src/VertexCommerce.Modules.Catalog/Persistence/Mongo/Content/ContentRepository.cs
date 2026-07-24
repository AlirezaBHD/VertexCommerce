using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;

internal sealed class ContentRepository(IMongoDatabase database) : IContentRepository
{
    private readonly IMongoCollection<HeroContentDocument> _heroes =
        database.GetCollection<HeroContentDocument>("hero_contents");

    private readonly IMongoCollection<BannerDocument> _banners =
        database.GetCollection<BannerDocument>("banners");

    private readonly IMongoCollection<AboutDocument> _about =
        database.GetCollection<AboutDocument>("about_content");

    private readonly IMongoCollection<ContactDocument> _contact =
        database.GetCollection<ContactDocument>("contact_content");

    // ── Hero ─────────────────────────────────────────────────────────────────

    public async Task<HeroContentDocument?> GetActiveHeroAsync(CancellationToken ct = default)
        => await _heroes.Find(h => h.IsActive).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<HeroContentDocument>> GetAllHeroAsync(CancellationToken ct = default)
        => await _heroes.Find(FilterDefinition<HeroContentDocument>.Empty)
            .SortByDescending(h => h.IsActive)
            .ThenByDescending(h => h.UpdatedAt)
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

    public async Task UpdateBannerSortOrderAsync(Guid id, int sortOrder, CancellationToken ct = default)
        => await _banners.UpdateOneAsync(
            b => b.Id == id,
            Builders<BannerDocument>.Update
                .Set(b => b.SortOrder, sortOrder)
                .Set(b => b.UpdatedAt, DateTime.UtcNow),
            cancellationToken: ct);

    // ── About ─────────────────────────────────────────────────────────────────

    public async Task<AboutDocument?> GetAboutAsync(CancellationToken ct = default)
        => await _about.Find(a => a.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"))
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAboutAsync(AboutDocument about, CancellationToken ct = default)
    {
        about.Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        about.UpdatedAt = DateTime.UtcNow;
        await _about.ReplaceOneAsync(
            a => a.Id == about.Id,
            about,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    // ── Contact ───────────────────────────────────────────────────────────────

    public async Task<ContactDocument?> GetContactAsync(CancellationToken ct = default)
        => await _contact.Find(c => c.Id == Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .FirstOrDefaultAsync(ct);

    public async Task UpsertContactAsync(ContactDocument contact, CancellationToken ct = default)
    {
        contact.Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        contact.UpdatedAt = DateTime.UtcNow;
        await _contact.ReplaceOneAsync(
            c => c.Id == contact.Id,
            contact,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }
}
