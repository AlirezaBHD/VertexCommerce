using VertexCommerce.Modules.Identity.Domain.Entities;

namespace VertexCommerce.Modules.Identity.Infrastructure.Authentication;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiry();
}
