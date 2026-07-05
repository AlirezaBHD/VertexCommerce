using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ConfirmOrder;

internal sealed class ConfirmOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork,
    IStockService stockService)
    : ICommandHandler<ConfirmOrderCommand>
{
    public async Task<Result> Handle(ConfirmOrderCommand command, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(Error.NotFound("Order", command.OrderId));

        var confirmResult = order.Confirm();
        if (confirmResult.IsFailure)
            return confirmResult;

        foreach (var item in order.Items)
        {
            var deductResult = await stockService.DeductStockAsync(item.VariantId, item.Quantity, ct);
            if (deductResult.IsFailure)
                return deductResult;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
