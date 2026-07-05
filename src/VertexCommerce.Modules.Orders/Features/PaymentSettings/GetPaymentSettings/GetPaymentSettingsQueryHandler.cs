using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Modules.Orders.Features.PaymentSettings;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettings;

internal sealed class GetAllPaymentSettingsQueryHandler(IPaymentSettingsRepository repository)
    : IQueryHandler<GetAllPaymentSettingsQuery, IReadOnlyList<PaymentSettingsResponse>>
{
    public async Task<Result<IReadOnlyList<PaymentSettingsResponse>>> Handle(
        GetAllPaymentSettingsQuery query, CancellationToken ct)
    {
        var all = await repository.GetAllAsync(ct);
        return Result.Success<IReadOnlyList<PaymentSettingsResponse>>(
            all.Select(s => s.ToResponse()).ToList());
    }
}

internal sealed class GetActivePaymentSettingsQueryHandler(IPaymentSettingsRepository repository)
    : IQueryHandler<GetActivePaymentSettingsQuery, PaymentSettingsResponse>
{
    public async Task<Result<PaymentSettingsResponse>> Handle(
        GetActivePaymentSettingsQuery query, CancellationToken ct)
    {
        var active = await repository.GetActiveAsync(ct);
        if (active is null)
            return Result.Failure<PaymentSettingsResponse>(
                Error.NotFound("PaymentSettings.Active", "No active payment settings found."));

        return Result.Success(active.ToResponse());
    }
}
