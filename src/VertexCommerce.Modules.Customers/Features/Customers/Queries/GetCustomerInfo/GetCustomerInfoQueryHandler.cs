using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Contracts.Customers;
using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerInfo;

internal sealed class GetCustomerInfoQueryHandler(CustomersDbContext context)
    : IQueryHandler<GetCustomerInfoQuery, CustomerInfoDto?>
{
    public async Task<Result<CustomerInfoDto?>> Handle(GetCustomerInfoQuery request, CancellationToken ct)
    {
        var spec = new GetCustomerInfoSpec(request.CustomerId);
        
        var customerInfo = await SpecificationEvaluator
            .ApplySpecification(context.Customers.AsNoTracking(), spec)
            .FirstOrDefaultAsync(ct);

        return Result.Success(customerInfo);
    }
}
