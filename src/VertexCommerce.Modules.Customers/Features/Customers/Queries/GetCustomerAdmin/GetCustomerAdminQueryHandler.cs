using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerAdmin;

internal sealed class GetCustomerAdminQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomerAdminQuery, CustomerAdminDetailResponse>
{
    public async Task<Result<CustomerAdminDetailResponse>> Handle(
        GetCustomerAdminQuery query,
        CancellationToken ct)
    {
        var spec = new GetCustomerAdminSpec(query.CustomerId);

        var customer = await customerRepository.GetAsync(spec, ct);

        if (customer is null)
        {
            return Result.Failure<CustomerAdminDetailResponse>(
                Error.NotFound("Customer", query.CustomerId));
        }

        return Result.Success(customer);
    }
}
