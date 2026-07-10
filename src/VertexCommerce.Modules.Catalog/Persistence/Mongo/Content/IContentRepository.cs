using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;

public interface IContentRepository
{
    // Hero
    Task<HeroContentDocument?> GetActiveHeroAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HeroContentDocument>> GetAllHeroAsync(CancellationToken ct = default);
    Task UpsertHeroAsync(HeroContentDocument hero, CancellationToken ct = default);
    Task SetActiveHeroAsync(Guid id, CancellationToken ct = default);
    Task DeleteHeroAsync(Guid id, CancellationToken ct = default);

    // Banners
    Task<IReadOnlyList<BannerDocument>> GetActiveBannersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BannerDocument>> GetAllBannersAsync(CancellationToken ct = default);
    Task<BannerDocument?> GetBannerByIdAsync(Guid id, CancellationToken ct = default);
    Task UpsertBannerAsync(BannerDocument banner, CancellationToken ct = default);
    Task DeleteBannerAsync(Guid id, CancellationToken ct = default);

    // About
    Task<AboutDocument?> GetAboutAsync(CancellationToken ct = default);
    Task UpsertAboutAsync(AboutDocument about, CancellationToken ct = default);

    // Contact
    Task<ContactDocument?> GetContactAsync(CancellationToken ct = default);
    Task UpsertContactAsync(ContactDocument contact, CancellationToken ct = default);
}
