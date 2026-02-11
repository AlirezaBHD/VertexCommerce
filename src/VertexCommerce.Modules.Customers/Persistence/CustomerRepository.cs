using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;

namespace VertexCommerce.Modules.Customers.Persistence;

internal sealed class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext _context;

    public CustomerRepository(CustomersDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Customers.AnyAsync(c => c.UserId == userId, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await _context.Customers.AddAsync(customer, ct);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }
}
