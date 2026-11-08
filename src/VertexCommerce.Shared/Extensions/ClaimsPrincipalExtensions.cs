using System.Security.Claims;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string SubjectClaim = "sub";
    private const string EmailClaim = "email";

    /// <exception cref="UnauthorizedException">No valid user id claim is present.</exception>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue(SubjectClaim);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedException("User id claim is missing or invalid.");
    }

    /// <exception cref="UnauthorizedException">No email claim is present.</exception>
    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email)
               ?? user.FindFirstValue(EmailClaim)
               ?? throw new UnauthorizedException("Email claim is missing.");
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(AppRoles.Admin);
    }
}
