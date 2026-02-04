namespace VertexCommerce.Shared.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.")
    {
    }
}
