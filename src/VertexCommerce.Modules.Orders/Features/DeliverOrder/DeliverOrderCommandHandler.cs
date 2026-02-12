using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.DeliverOrder;

internal sealed class DeliverOrderCommandHandler : ICommandHandler<DeliverOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public DeliverOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeliverOrderCommand command, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(Error.NotFound("Order", command.OrderId));

        var result = order.Deliver();
        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
