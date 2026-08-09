using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    public override int StatusCode => StatusCodes.Status422UnprocessableEntity;
    public override string ErrorCode { get; }

    public BusinessRuleException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
