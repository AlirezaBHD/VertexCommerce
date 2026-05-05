using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Basket.Configuration;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Endpoints;
using VertexCommerce.Modules.Basket.Features;
using VertexCommerce.Modules.Basket.GraphQL;
using VertexCommerce.Modules.Basket.GraphQL.Types;
using VertexCommerce.Modules.Basket.Persistence;
using VertexCommerce.Modules.Basket.Persistence.Configuration;
using VertexCommerce.Modules.Basket.Services;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Contracts.Baskets;

namespace VertexCommerce.Modules.Basket;

public class BasketModule : IModule
{
    public string Name => "Basket";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        MongoDbConfiguration.Configure();

        services.Configure<BasketSettings>(
            configuration.GetSection(BasketSettings.SectionName));
        services.Configure<MongoDbSettings>(
            configuration.GetSection($"{BasketSettings.SectionName}:MongoDB"));

        // Infrastructure
        // services.AddSingleton<MongoDbContext>();

        // Domain Services
        services.AddScoped<BasketFactory>();
        
        
        // services.Configure<MongoDbSettings>(
        //     configuration.GetSection("MongoDb"));

        services.AddScoped<IBasketRepository, BasketRepository>();

        services.AddScoped<IBasketService, BasketService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(BasketModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(BasketModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapBasketEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
        builder.AddTypeExtension<BasketQueries>();
    }
}
