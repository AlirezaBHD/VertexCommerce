using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;
using DomainEntity = VertexCommerce.Modules.Orders.Domain.Entities.PaymentSettings;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;

internal sealed class CreatePaymentSettingsCommandHandler(
    IPaymentSettingsRepository repository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<CreatePaymentSettingsCommand, PaymentSettingsResponse>
{
    public async Task<Result<PaymentSettingsResponse>> Handle(CreatePaymentSettingsCommand command, CancellationToken ct)
    {
        var settings = DomainEntity.Create(
            bankName: command.BankName,
            accountHolderName: command.AccountHolderName,
            cardNumber: command.CardNumber,
            shabaNumber: command.ShabaNumber,
            accountNumber: command.AccountNumber,
            description: command.Description);

        await repository.AddAsync(settings, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(settings.ToResponse());
    }
}
