using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.DeletePaymentSettings;

internal sealed class DeletePaymentSettingsCommandHandler(
    IPaymentSettingsRepository repository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<DeletePaymentSettingsCommand>
{
    public async Task<Result> Handle(DeletePaymentSettingsCommand command, CancellationToken ct)
    {
        var settings = await repository.GetByIdAsync(command.Id, ct);
        if (settings is null)
            return Result.Failure(Error.NotFound("PaymentSettings", command.Id));

        if (settings.IsActive)
            return Result.Failure(Error.Validation("PaymentSettings.ActiveCannotDelete",
                "Cannot delete the active payment settings. Set another one as active first."));

        repository.Delete(settings);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
