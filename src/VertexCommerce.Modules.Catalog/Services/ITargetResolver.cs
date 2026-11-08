using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Services;

public interface ITargetResolver
{
    string? ResolveHref(BannerTarget target, out bool isExternal);
}
