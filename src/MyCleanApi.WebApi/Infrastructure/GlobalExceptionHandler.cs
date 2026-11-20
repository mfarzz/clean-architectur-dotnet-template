using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCleanApi.Application.Common.Exceptions;

namespace MyCleanApi.WebApi.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new HttpValidationProblemDetails(validationEx.Errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred."
                }),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message
                })
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
