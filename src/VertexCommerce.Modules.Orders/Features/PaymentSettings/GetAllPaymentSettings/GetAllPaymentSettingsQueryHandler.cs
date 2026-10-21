using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetAllPaymentSettings;

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