using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Shared.Contracts.Identity;

/// <summary>Identity of the user making the current request.</summary>
public interface ICurrentUser
{
    /// <summary>Returns the authenticated user's id.</summary>
    /// <exception cref="UnauthorizedException">The request is not authenticated or carries no valid user id claim.</exception>
    Guid GetUserId();
}
