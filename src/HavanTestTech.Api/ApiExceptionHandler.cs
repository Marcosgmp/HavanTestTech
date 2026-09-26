using HavanTestTech.Application.Exceptions;
using HavanTestTech.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HavanTestTech.Api;

/// <summary>
/// Single place that translates exceptions into HTTP responses (RFC 7807 problem details),
/// so endpoints do not need their own try/catch blocks.
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            DomainException or InvalidRequestException => (StatusCodes.Status400BadRequest, "Validation error"),
            TodoNotFoundException => (StatusCodes.Status404NotFound, "Task not found"),
            BadHttpRequestException badRequest => (badRequest.StatusCode, "Invalid request"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        var isUnexpected = status == StatusCodes.Status500InternalServerError;
        if (isUnexpected)
        {
            logger.LogError(exception, "Unhandled exception on {Path}", httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                // Internal error details are never sent to clients.
                Detail = isUnexpected ? null : exception.Message
            },
            cancellationToken);

        return true;
    }
}