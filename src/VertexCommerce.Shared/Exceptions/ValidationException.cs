using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

public sealed class ValidationException : DomainException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string ErrorCode => "VALIDATION_ERROR";
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
