using System.Security.Claims;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public Guid GetUserId()
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("sub");

            if (claim is null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId;
        }

        public string GetEmail()
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                   ?? user.FindFirst("email")?.Value
                   ?? throw new UnauthorizedAccessException("Email not found in token");
        }

        public bool IsAdmin()
        {
            return user.IsInRole(AppRoles.Admin);
        }
    }
}
