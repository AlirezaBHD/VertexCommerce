using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings.GetShippingSettings;

internal sealed class GetShippingSettingsQueryHandler(IShippingSettingsRepository repository)
    : IQueryHandler<GetShippingSettingsQuery, ShippingSettingsResponse>
{
    public async Task<Result<ShippingSettingsResponse>> Handle(
        GetShippingSettingsQuery query, CancellationToken ct)
    {
        var settings = await repository.GetAsync(ct);
        if (settings is null)
        {
            return Result.Success(ShippingSettingsMapper.ToDefaultResponse());
        }

        return Result.Success(settings.ToResponse());
    }
}
