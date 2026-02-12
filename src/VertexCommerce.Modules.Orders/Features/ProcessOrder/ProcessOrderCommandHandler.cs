using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ProcessOrder;

internal sealed class ProcessOrderCommandHandler : ICommandHandler<ProcessOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public ProcessOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ProcessOrderCommand command, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(Error.NotFound("Order", command.OrderId));

        var result = order.StartProcessing();
        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
