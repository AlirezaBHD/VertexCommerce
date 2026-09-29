using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Endpoints;
using VertexCommerce.Modules.Customers.Infrastructure.Services;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Modules.Customers.Persistence.Repositories;
using VertexCommerce.Modules.Customers.Services;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Customers;

public interface ICustomersModule { }

public class CustomersModule : IModule
{
    public string Name => "Customers";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CustomersDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "customers")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        
        services.AddScoped<ICustomerUnitOfWork>(sp =>
            sp.GetRequiredService<CustomersDbContext>());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CustomersModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(CustomersModule).Assembly);
        
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerResolver, CustomerResolver>();
        services.Decorate<ICustomerResolver, CachedCustomerResolver>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCustomerEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
        
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CustomersModule>>();

        try
        {
            logger.LogInformation("Applying migrations for Customers module...");
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Customers migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Customers module.");
            throw;
        }
    }
}
