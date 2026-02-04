using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CatalogDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        // MediatR handlers from this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);

        return services;
    }
}
