using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.UpdatePaymentSettings;

internal sealed class UpdatePaymentSettingsCommandHandler(
    IPaymentSettingsRepository repository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<UpdatePaymentSettingsCommand, PaymentSettingsResponse>
{
    public async Task<Result<PaymentSettingsResponse>> Handle(UpdatePaymentSettingsCommand command, CancellationToken ct)
    {
        var settings = await repository.GetByIdAsync(command.Id, ct);
        if (settings is null)
            return Result.Failure<PaymentSettingsResponse>(Error.NotFound("PaymentSettings", command.Id));

        settings.Update(
            bankName: command.BankName,
            accountHolderName: command.AccountHolderName,
            cardNumber: command.CardNumber,
            shabaNumber: command.ShabaNumber,
            accountNumber: command.AccountNumber,
            description: command.Description);

        repository.Update(settings);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(settings.ToResponse());
    }
}
