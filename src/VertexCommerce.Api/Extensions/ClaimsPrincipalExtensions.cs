using System.Security.Claims;

namespace VertexCommerce.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? user.FindFirst("sub");

        if (claim is null || !Guid.TryParse(claim.Value, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");

        return userId;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value
               ?? user.FindFirst("email")?.Value
               ?? throw new UnauthorizedAccessException("Email not found in token");
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }
}
