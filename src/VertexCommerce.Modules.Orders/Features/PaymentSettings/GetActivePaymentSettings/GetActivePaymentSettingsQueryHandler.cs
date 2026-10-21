using VertexCommerce.Modules.Orders.Domain.Errors;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetActivePaymentSettings;

internal sealed class GetActivePaymentSettingsQueryHandler(IPaymentSettingsRepository repository)
    : IQueryHandler<GetActivePaymentSettingsQuery, PaymentSettingsResponse>
{
    public async Task<Result<PaymentSettingsResponse>> Handle(
        GetActivePaymentSettingsQuery query, CancellationToken ct)
    {
        var active = await repository.GetActiveAsync(ct);
        if (active is null)
            return Result.Failure<PaymentSettingsResponse>(
                PaymentSettingsErrors.NoActiveSettings);

        return Result.Success(active.ToResponse());
    }
}
