using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed class GetAttributesSpec : BaseSpecification<CatalogAttribute, CatalogAttributesResponse>
{
    public GetAttributesSpec()
    {
        Include(a => a.Options);
        Select(a => new CatalogAttributesResponse(
            Code: a.Code,
            DefaultName: a.DefaultName,
            Options: MapOptions(a.Options)
        ));
    }

    private static List<CatalogAttributeOptionResponse> MapOptions(IEnumerable<CatalogAttributeOption> options) =>
        options.Select(o => new CatalogAttributeOptionResponse(
            o.Code,
            o.DefaultName,
            o.MediaPath
            )).ToList();
}
