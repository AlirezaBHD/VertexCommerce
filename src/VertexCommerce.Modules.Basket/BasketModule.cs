using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Modules.Basket.Persistence;
using VertexCommerce.Modules.Basket.Services;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Modules.Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MongoDB Configuration
        MongoDbConfiguration.Configure();

        // Settings
        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDb"));

        // Repository
        services.AddSingleton<IBasketRepository, BasketRepository>();

        // Cross-Module Service
        services.AddScoped<IBasketService, BasketService>();

        // MediatR handlers from this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(BasketModule).Assembly));

        // FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(BasketModule).Assembly);

        return services;
    }
}
