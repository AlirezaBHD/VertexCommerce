using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.InitiatePayment;

public sealed class InitiatePaymentCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : ICommandHandler<InitiatePaymentCommand>
{
    public async Task<Result> Handle(InitiatePaymentCommand command, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var order = await orderRepository.GetByIdAsync(command.OrderId, ct);

        if (order is null)
        {
            return Result.Failure(
                Error.NotFound("Order", command.OrderId.ToString()));
        }

        if (order.CustomerId != customerId)
        {
            return Result.Failure(
                Error.NotFound("Order for Customer", command.OrderId.ToString()));
        }

        order.InitiatePayment();

        await unitOfWork.SaveChangesAsync(ct);


        return Result.Success();
    }
}
