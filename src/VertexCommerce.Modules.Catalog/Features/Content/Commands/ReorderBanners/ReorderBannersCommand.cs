using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.ReorderBanners;

public sealed record ReorderBannersCommand(IList<ReorderBannerItem> Items) : ICommand;
