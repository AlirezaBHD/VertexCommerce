using HotChocolate;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content;

[ExtendObjectType("Query")]
public sealed class ContentQueries
{
    public static async Task<IReadOnlyList<HeroContentDocument>> GetHeroItems(
        [Service] IContentRepository repository,
        CancellationToken ct)
        => await repository.GetAllHeroAsync(ct);

    public static async Task<IReadOnlyList<BannerDocument>> GetBannerItems(
        [Service] IContentRepository repository,
        CancellationToken ct)
        => await repository.GetAllBannersAsync(ct);

    public static async Task<AboutDocument?> GetAboutContent(
        [Service] IContentRepository repository,
        CancellationToken ct)
        => await repository.GetAboutAsync(ct);

    public static async Task<ContactDocument?> GetContactContent(
        [Service] IContentRepository repository,
        CancellationToken ct)
        => await repository.GetContactAsync(ct);
}
