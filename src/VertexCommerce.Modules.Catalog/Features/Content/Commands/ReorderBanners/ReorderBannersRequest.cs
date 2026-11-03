namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.ReorderBanners;

public sealed record ReorderBannersRequest(IList<ReorderBannerItem> Items);
