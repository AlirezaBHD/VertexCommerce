using VertexCommerce.Modules.Identity.Domain.Entities;

namespace VertexCommerce.Modules.Identity.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiry();
}
