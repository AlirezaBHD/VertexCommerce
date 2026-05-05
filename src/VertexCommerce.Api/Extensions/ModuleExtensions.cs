using HotChocolate.Execution.Configuration;
using VertexCommerce.Modules.Catalog;
using VertexCommerce.Modules.Basket;
using VertexCommerce.Modules.Customers;
using VertexCommerce.Modules.Identity;
using VertexCommerce.Modules.Orders;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Api.Extensions;

public static class ModuleExtensions
{
    private static readonly List<IModule> Modules = new()
    {
        new BasketModule(),
        new CatalogModule(),
        new CustomersModule(),
        new IdentityModule(),
        new OrdersModule()
    };

    public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in Modules)
        {
            module.RegisterServices(services, configuration);
        }
        return services;
    }

    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in Modules)
        {
            module.MapEndpoints(endpoints);
        }
        return endpoints;
    }

    public static IRequestExecutorBuilder ConfigureModulesGraphQl(this IRequestExecutorBuilder builder)
    {
        foreach (var module in Modules)
        {
            module.ConfigureGraphQl(builder);
        }
        return builder;
    }

    public static async Task InitializeModulesAsync(this IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        foreach (var module in Modules)
        {
            await module.InitializeAsync(serviceProvider, ct);
        }
    }
}
