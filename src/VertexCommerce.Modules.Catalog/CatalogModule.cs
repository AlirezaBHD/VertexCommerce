using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Endpoints;
using VertexCommerce.Modules.Catalog.GraphQL;
using VertexCommerce.Modules.Catalog.GraphQL.Content;
using VertexCommerce.Modules.Catalog.GraphQL.Content.Types;
using VertexCommerce.Modules.Catalog.GraphQL.HomePage.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;
using VertexCommerce.Modules.Catalog.Services;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Categories;
using VertexCommerce.Modules.Catalog.Sync.Products;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.Persistence;
using VertexCommerce.Shared.Persistence.Outbox;
using VertexCommerce.Shared.Persistence.Outbox.Extensions;

namespace VertexCommerce.Modules.Catalog;

public class CatalogModule : IModule
{
    public string Name => "Catalog";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventInterceptor>();

        services.AddOutbox<CatalogDbContext>(opts =>
        {
            opts.ModuleName = Name;
            opts.BatchSize = 25;
            opts.MaxRetryCount = 3;
            opts.PollingInterval = TimeSpan.FromSeconds(30);
        });

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("CatalogDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog"));

            options.AddInterceptors(sp.GetRequiredService<OutboxInterceptor>());
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICatalogUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddSingleton<ProductIndexManager>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IProductReadModelRepository, ProductReadModelRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IMediaFileRepository, MediaFileRepository>();

        services.AddScoped<IProductSyncService, ProductSyncService>();
        services.AddScoped<CategoryPathBuilder>();

        services.AddSingleton<CategoryIndexManager>();
        services.AddScoped<ICategoryReadModelRepository, CategoryReadModelRepository>();
        services.AddScoped<CategorySyncService>();

        // Banner
        services.AddSingleton<BannerIndexManager>();
        services.AddScoped<IBannerService, BannerService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapEndpoints();
        endpoints.MapContentEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
        builder.AddTypeExtension<CatalogQueries>();
        builder.AddTypeExtension<ContentQueries>();
        builder.AddType<HomePageType>();
        builder.AddType<HeroContentType>();
        builder.AddType<BannerType>();
        builder.AddType<AboutType>();
        builder.AddType<ContactType>();
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var productRepo = serviceProvider.GetRequiredService<ProductIndexManager>();
        await productRepo.EnsureIndexesAsync(ct);

        var categoryRepo = serviceProvider.GetRequiredService<CategoryIndexManager>();
        await categoryRepo.EnsureIndexesAsync(ct);

        var bannerRepo = serviceProvider.GetRequiredService<BannerIndexManager>();
        await bannerRepo.EnsureIndexesAsync(ct);
    }
}
