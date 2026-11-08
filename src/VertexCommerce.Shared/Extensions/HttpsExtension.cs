using Microsoft.AspNetCore.Http;
using Error = VertexCommerce.Shared.CQRS.Error;
namespace VertexCommerce.Shared.Extensions;

public static class HttpsExtension
{
    public static int GetStatusCode(string errorCode) => errorCode switch
    {
        _ when errorCode.Contains("NotFound", StringComparison.Ordinal) => StatusCodes.Status404NotFound,
        _ when errorCode.Contains("Validation", StringComparison.Ordinal) => StatusCodes.Status400BadRequest,
        _ when errorCode.Contains("Conflict", StringComparison.Ordinal) => StatusCodes.Status409Conflict,
        _ when errorCode.Contains("Authentication", StringComparison.Ordinal) => StatusCodes.Status400BadRequest,
        _ when errorCode.Contains("Error.NullValue", StringComparison.Ordinal) => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
    
    public static IResult ToHttpResult(this Error error)
    {
        return Results.Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: GetStatusCode(error.Code));
    }
}
