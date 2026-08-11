using Microsoft.AspNetCore.Http;
using Error = VertexCommerce.Shared.CQRS.Error;
namespace VertexCommerce.Shared.Extensions;

public static class HttpsExtension
{
    public static int GetStatusCode(string errorCode) => errorCode switch
    {
        _ when errorCode.Contains("NotFound") => StatusCodes.Status404NotFound,
        _ when errorCode.Contains("Validation") => StatusCodes.Status400BadRequest,
        _ when errorCode.Contains("Conflict") => StatusCodes.Status409Conflict,
        _ when errorCode.Contains("Authentication") => StatusCodes.Status400BadRequest,
        _ when errorCode.Contains("Error.NullValue") => StatusCodes.Status400BadRequest,
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
