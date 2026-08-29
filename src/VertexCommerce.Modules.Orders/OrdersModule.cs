using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Endpoints;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Modules.Orders;

public class OrdersModule :IModule
{
    public string Name => "Orders";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("OrdersDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "orders")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentSettingsRepository, PaymentSettingsRepository>();
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
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
        
    }
}
