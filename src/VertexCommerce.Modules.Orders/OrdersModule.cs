using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("OrdersDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "orders")));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrdersDbContext>());

        // MediatR handlers from this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(OrdersModule).Assembly));

        // FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly);

        return services;
    }
}
