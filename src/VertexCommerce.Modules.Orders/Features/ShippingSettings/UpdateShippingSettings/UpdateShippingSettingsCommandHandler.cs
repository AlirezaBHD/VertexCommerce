using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings.UpdateShippingSettings;

internal sealed class UpdateShippingSettingsCommandHandler(
    IShippingSettingsRepository repository,
    IOrdersUnitOfWork unitOfWork)
    : ICommandHandler<UpdateShippingSettingsCommand, ShippingSettingsResponse>
{
    public async Task<Result<ShippingSettingsResponse>> Handle(
        UpdateShippingSettingsCommand command, CancellationToken ct)
    {
        var currency = string.IsNullOrWhiteSpace(command.Currency) ? "USD" : command.Currency;
        var costMoney = Money.Create(command.Cost, currency);
        var thresholdMoney = command.FreeShippingThreshold.HasValue && command.FreeShippingThreshold.Value > 0
            ? Money.Create(command.FreeShippingThreshold.Value, currency)
            : (Money?)null;

        var existing = await repository.GetAsync(ct);
        if (existing is null)
        {
            var newSettings = Domain.Entities.ShippingSettings.Create(
                costMoney,
                thresholdMoney,
                command.Description,
                command.IsActive);

            await repository.AddAsync(newSettings, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result.Success(newSettings.ToResponse());
        }

        existing.Update(
            costMoney,
            thresholdMoney,
            command.Description,
            command.IsActive);

        repository.Update(existing);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(existing.ToResponse());
    }
}
