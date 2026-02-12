using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ConfirmOrder;

internal sealed class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public ConfirmOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmOrderCommand command, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(Error.NotFound("Order", command.OrderId));

        var result = order.Confirm();
        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
