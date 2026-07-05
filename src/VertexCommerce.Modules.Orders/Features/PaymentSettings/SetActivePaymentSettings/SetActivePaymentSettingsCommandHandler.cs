using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.SetActivePaymentSettings;

internal sealed class SetActivePaymentSettingsCommandHandler(
    IPaymentSettingsRepository repository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<SetActivePaymentSettingsCommand>
{
    public async Task<Result> Handle(SetActivePaymentSettingsCommand command, CancellationToken ct)
    {
        var target = await repository.GetByIdAsync(command.Id, ct);
        if (target is null)
            return Result.Failure(Error.NotFound("PaymentSettings", command.Id));

        var current = await repository.GetActiveAsync(ct);
        if (current is not null && current.Id != command.Id)
        {
            current.Deactivate();
            repository.Update(current);
        }

        target.Activate();
        repository.Update(target);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
