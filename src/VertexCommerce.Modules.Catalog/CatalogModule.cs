using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Categories;
using VertexCommerce.Modules.Catalog.Sync.Products;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("CatalogDb"),
                npgsql => npgsql.MigrationsHistoryTable(
                    "__EFMigrationsHistory", "catalog"));

            options.AddInterceptors(
                sp.GetRequiredService<DomainEventInterceptor>());
        });
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICatalogUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddSingleton<ProductIndexManager>();
        services.AddScoped<IProductReadModelRepository, ProductReadModelRepository>();
        
        services.AddScoped<IProductSyncService, ProductSyncService>();
        services.AddScoped<CategoryPathBuilder>();
        
        services.AddSingleton<CategoryIndexManager>();
        services.AddScoped<ICategoryReadModelRepository, CategoryReadModelRepository>();
        services.AddScoped<CategorySyncService>();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);

        return services;
    }

    public static async Task InitializeCatalogIndexesAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        var productRepo = serviceProvider.GetRequiredService<ProductIndexManager>();
        await productRepo.EnsureIndexesAsync(ct);
        
        var categoryRepo = serviceProvider.GetRequiredService<CategoryIndexManager>();
        await categoryRepo.EnsureIndexesAsync(ct);
    }
}