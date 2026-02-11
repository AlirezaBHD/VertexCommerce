using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;

namespace VertexCommerce.Modules.Customers;

public interface ICustomersModule { }

public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CustomersDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "customers")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerUnitOfWork>(sp =>
            sp.GetRequiredService<CustomersDbContext>());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CustomersModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CustomersModule).Assembly);

        return services;
    }
}
