using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Orders.Persistence;

namespace VertexCommerce.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        app.ApplyMigrations<CustomersDbContext>();
        app.ApplyMigrations<CatalogDbContext>();
        app.ApplyMigrations<IdentityDbContext>();
        app.ApplyMigrations<OrdersDbContext>();
    }

    private static void ApplyMigrations<TContext>(this IApplicationBuilder app)
        where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        db.Database.Migrate();
    }
}