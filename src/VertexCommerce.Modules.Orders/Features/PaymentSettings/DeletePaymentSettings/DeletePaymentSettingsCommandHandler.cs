using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;

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
            return Result.Failure(PaymentSettingsErrors.NotFound(command.Id));

        if (settings.IsActive)
            return Result.Failure(PaymentSettingsErrors.ActiveCannotDelete);

        repository.Delete(settings);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
