using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

internal sealed class GetCustomerQueryHandler(ICustomerRepository customerRepository, ICurrentUser currentUser)
    : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    public async Task<Result<CustomerResponse>> Handle(GetCustomerQuery query, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var spec = new GetCustomerSpec(userId: userId);

        var customer = await customerRepository.GetAsync(spec, ct);

        if (customer is null)
        {
            return Result.Failure<CustomerResponse>(Error.NotFound("Customer", userId));
        }

        return Result.Success(customer);
    }
}
