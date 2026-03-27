using VertexCommerce.Api.GraphQL;
using VertexCommerce.Api.GraphQL.Basket;
using VertexCommerce.Api.GraphQL.Catalog;
using VertexCommerce.Api.GraphQL.Catalog.Categories;
using VertexCommerce.Api.GraphQL.Catalog.Types;
using VertexCommerce.Api.GraphQL.Orders;

namespace VertexCommerce.Api.Extensions;

public static class GraphQLExtensions
{
    public static IServiceCollection AddVertexGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddTypeExtension<OrderQueries>()
            .AddTypeExtension<BasketQueries>()
            .AddTypeExtension<CatalogQueries>()
            .AddType<CategoryType>()
            .AddType<ProductType>()
            .AddMongoDbPagingProviders() 
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddMongoDbSorting();

        return services;
    }
}
