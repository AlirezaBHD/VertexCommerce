using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders.Features.CancelOrder;

public sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelOrderCommand command, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, ct);

        if (order is null)
        {
            return Result.Failure(Error.NotFound("Order", command.OrderId));
        }

        try
        {
            order.Cancel(command.Reason);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation(ex.Message));
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
