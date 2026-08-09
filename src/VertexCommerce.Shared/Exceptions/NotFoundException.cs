using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

public sealed class NotFoundException : DomainException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string ErrorCode => "NOT_FOUND";

    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.")
    {
    }
}
