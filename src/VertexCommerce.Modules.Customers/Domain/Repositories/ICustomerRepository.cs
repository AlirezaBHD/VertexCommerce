using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Services;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TResult?> GetAsync<TResult>(ISpecification<Customer, TResult> spec,
        CancellationToken ct = default);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> SearchAsync(string? searchTerm, int limit = 20, CancellationToken ct = default);
    Task<PagedResult<TResult>> GetPaginatedAsync<TResult>(ISpecification<Customer, TResult> spec,
        int skip = 0, int take = 10, CancellationToken ct = default);
    Task<Guid> GetIdByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
    void Update(Customer customer);
    Task<CustomerInfoDto?> GetCustomerInfoAsync(GetCustomerInfoSpec spec, CancellationToken ct);
}
