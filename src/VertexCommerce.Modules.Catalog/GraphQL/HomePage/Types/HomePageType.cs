using VertexCommerce.Modules.Catalog.GraphQL.Categories.Types;
using VertexCommerce.Modules.Catalog.GraphQL.Products.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

namespace VertexCommerce.Modules.Catalog.GraphQL.HomePage.Types;

public sealed class HomePageType : ObjectType<HomePageData>
{
    protected override void Configure(IObjectTypeDescriptor<HomePageData> descriptor)
    {
        descriptor.Name("HomePage");

        descriptor
            .Field("featuredCategories")
            .Description("Categories marked to show on the home page")
            .Type<NonNullType<ListType<NonNullType<CategoryType>>>>()
            .UseProjection()
            .Resolve(ctx =>
            {
                var repo = ctx.Service<ICategoryReadModelRepository>();
                return repo.GetFilteredCategories(isActive: true, showOnHome: true, showOnMenu: null);
            });

        descriptor
            .Field("newArrivals")
            .Description("Latest 10 active products")
            .Type<NonNullType<ListType<NonNullType<ProductType>>>>()
            .UseProjection()
            .Resolve(ctx =>
            {
                var repo = ctx.Service<IProductReadModelRepository>();
                return repo.GetLatestProducts(limit: 10);
            });
    }
}
