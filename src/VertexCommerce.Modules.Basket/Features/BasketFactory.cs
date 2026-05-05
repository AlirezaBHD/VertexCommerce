using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Basket.Configuration;
using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Features;

internal sealed class BasketFactory(
    IOptions<BasketSettings> settings)
{
    private readonly BasketSettings _settings = settings.Value;

    public BasketDocument CreateNew(Guid customerId) => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = customerId,
        Items = [],
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes)
    };

    public void RefreshExpiration(BasketDocument basket)
    {
        basket.UpdatedAt = DateTime.UtcNow;
        basket.ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes);
    }
}
