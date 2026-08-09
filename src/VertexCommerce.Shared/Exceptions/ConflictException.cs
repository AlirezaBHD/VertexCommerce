using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

public sealed class ConflictException : DomainException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string ErrorCode => "CONFLICT";

    public ConflictException(string message) : base(message)
    {
    }
}
