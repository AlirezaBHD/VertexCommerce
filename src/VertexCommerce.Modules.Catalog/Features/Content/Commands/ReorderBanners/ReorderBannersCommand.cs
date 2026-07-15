using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.ReorderBanners;

public sealed record ReorderBannerItem(Guid BannerId, int SortOrder);

public sealed record ReorderBannersRequest(IList<ReorderBannerItem> Items);

public sealed record ReorderBannersCommand(IList<ReorderBannerItem> Items) : ICommand;
