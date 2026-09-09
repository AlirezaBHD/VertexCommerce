using Microsoft.AspNetCore.Http;

namespace VertexCommerce.Shared.Exceptions;

/// <summary>
/// Thrown when a value violates the invariant of the domain concept it is being assigned to,
/// for example constructing a value object from a malformed or out-of-range value.
/// </summary>
/// <remarks>
/// This is the domain's last line of defence, not the primary validation channel.
/// Caller input is expected to be rejected earlier by FluentValidation at the API boundary,
/// which yields a structured 400 listing every offending field. Reaching this exception means
/// either an unvalidated code path or an internal caller violating the contract.
/// </remarks>
public sealed class DomainValidationException : DomainException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string ErrorCode { get; }

    /// <summary>The domain concept that rejected the value (e.g. "PostalCode").</summary>
    public string Field { get; }

    public DomainValidationException(string field, string message) : base(message)
    {
        Field = field;
        ErrorCode = $"{field}.Invalid";
    }
}
