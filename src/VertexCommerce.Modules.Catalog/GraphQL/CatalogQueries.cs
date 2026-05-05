using System.ComponentModel;
using HotChocolate;
using HotChocolate.CostAnalysis.Types;
using VertexCommerce.Modules.Catalog.GraphQL.HomePage;
using VertexCommerce.Modules.Catalog.GraphQL.HomePage.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL;

[ExtendObjectType("Query")]
public sealed class CatalogQueries
{
    [UseProjection]
    [UseFirstOrDefault]
    public IExecutable<ProductReadModel> GetProductBySlug(
        string slug,
        [Service] IProductReadModelRepository repository)
    {
        return repository.GetBySlugAsync(slug);
    }
    
    [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 50)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IExecutable<ProductReadModel> GetProducts(
        [Service] IProductReadModelRepository repository)
    {
        return repository.GetAll();
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Cost(10)]
    public IExecutable<CategoryReadModel> GetCategories(
        [Service] ICategoryReadModelRepository repository,
        bool? isActive,
        CancellationToken ct)
    {
        return repository.GetAllAsync(isActive, ct);
    }

    [UseProjection]
    public IExecutable<CategoryReadModel> GetCategoryById(
        [Service] ICategoryReadModelRepository repository,
        Guid id,
        CancellationToken ct)
    {
        return repository.GetByIdAsync(id, ct);
    }

    [GraphQLType(typeof(HomePageType))]
    [Description("Gets all aggregated data required for the home page layout")]
    public HomePageData GetHomePage()
    {
        return new HomePageData();
    }
}
