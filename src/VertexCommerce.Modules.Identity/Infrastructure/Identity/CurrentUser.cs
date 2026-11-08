using Microsoft.AspNetCore.Http;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Exceptions;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Identity.Infrastructure.Identity;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User
                   ?? throw new UnauthorizedException("No HTTP context is available for the current request.");

        return user.GetUserId();
    }
}
