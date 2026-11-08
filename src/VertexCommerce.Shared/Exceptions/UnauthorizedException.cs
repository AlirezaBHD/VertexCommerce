using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

/// <summary>
/// Thrown when the current request has no authenticated identity, or its token lacks a required claim.
/// Mapped to <c>401 Unauthorized</c> by the global exception handler.
/// </summary>
public sealed class UnauthorizedException : DomainException
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string ErrorCode => "UNAUTHORIZED";

    public UnauthorizedException(string message) : base(message)
    {
    }
}
