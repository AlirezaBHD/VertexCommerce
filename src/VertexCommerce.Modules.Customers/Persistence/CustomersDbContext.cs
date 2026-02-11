using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Persistence;

public sealed class CustomersDbContext : DbContext, ICustomerUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
