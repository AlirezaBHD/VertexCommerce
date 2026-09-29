using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Endpoints;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders;

public class OrdersModule :IModule
{
    public string Name => "Orders";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<OrdersDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("OrdersDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "orders"));

            options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentSettingsRepository, PaymentSettingsRepository>();
        services.AddScoped<IShippingSettingsRepository, ShippingSettingsRepository>();
        services.AddScoped<IOrdersUnitOfWork>(sp => sp.GetRequiredService<OrdersDbContext>());

        services.AddHostedService<BackgroundServices.OrderExpirationBackgroundService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(OrdersModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCheckoutEndpoints();
        endpoints.MapOrdersEndpoints();
        endpoints.MapPaymentSettingsEndpoints();
        endpoints.MapShippingSettingsEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
        
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<OrdersModule>>();

        try
        {
            logger.LogInformation("Applying migrations for Orders module...");
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Orders migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Orders module.");
            throw;
        }
    }
}
