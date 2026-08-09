using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Api.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(httpContext, exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var (statusCode, title, errorCode, extensions) = exception switch
        {
            DomainException domainEx => (
                domainEx.StatusCode,
                domainEx.Message,
                domainEx.ErrorCode,
                domainEx.Extensions
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An internal server error occurred.",
                "INTERNAL_ERROR",
                null as Dictionary<string, object>
            )
        };

        switch (statusCode)
        {
            case >= 500:
                logger.LogError(exception, "Server error: {Message}", exception.Message);
                break;
            case >= 400:
                logger.LogWarning(exception, "Client error: {Message}", exception.Message);
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Instance = $"{context.Request.Method} {context.Request.Path}",
            Extensions =
            {
                ["errorCode"] = errorCode,
                ["traceId"] = context.TraceIdentifier
            }
        };

        if (Activity.Current?.Id is not null)
        {
            problemDetails.Extensions["requestId"] = Activity.Current.Id;
        }

        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        if (extensions is not null)
        {
            foreach (var (key, value) in extensions)
            {
                problemDetails.Extensions.TryAdd(key, value);
            }
        }

        if (exception is ValidationException validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Errors;
        }

        return problemDetails;
    }
}
