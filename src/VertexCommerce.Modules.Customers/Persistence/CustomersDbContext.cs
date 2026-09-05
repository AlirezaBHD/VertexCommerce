using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Customers.Persistence;

public sealed class CustomersDbContext : DbContext, ICustomerUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new StringFieldConvention());
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
