using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Features.PaymentSettings.CreatePaymentSettings;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.PaymentSettings.GetPaymentSettingById;

internal sealed class GetPaymentSettingByIdQueryHandler(IPaymentSettingsRepository repository)
    : IQueryHandler<GetPaymentSettingByIdQuery, PaymentSettingsResponse>
{
    public async Task<Result<PaymentSettingsResponse>> Handle(
        GetPaymentSettingByIdQuery query, CancellationToken ct)
    {
        var result = await repository.GetByIdAsync(query.Id, ct);
        if (result == null)
        {
            return Result.Failure<PaymentSettingsResponse>(Error.NotFound("Payment setting", query.Id));
        }
        return Result.Success(
            result.ToResponse());
    }
}