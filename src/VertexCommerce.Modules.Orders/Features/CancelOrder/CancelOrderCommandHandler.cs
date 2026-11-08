using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;

namespace VertexCommerce.Modules.Orders.Features.CancelOrder;

internal sealed class CancelOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<CancelOrderCommand>
{
    public async Task<Result> Handle(CancelOrderCommand command, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(command.OrderId));

        var result = order.Cancel(command.Reason);
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
