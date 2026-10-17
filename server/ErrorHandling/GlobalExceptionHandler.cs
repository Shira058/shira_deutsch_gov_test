using Microsoft.AspNetCore.Diagnostics;
using RA_MuniAnnouncements.Exceptions;

namespace RA_MuniAnnouncements.ErrorHandling;

/// <summary>
/// Single place that turns exceptions into ProblemDetails responses.
/// Details stay in the log; the client only gets a generic, safe message.
/// </summary>
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case AuthorityNotFoundException notFound:
                logger.LogInformation("No settings for authority {AuthorityCode}", notFound.AuthorityCode);
                return await WriteProblemAsync(httpContext, exception, StatusCodes.Status404NotFound,
                    "Authority not found", $"No settings exist for authority '{notFound.AuthorityCode}'.");

            case UpstreamUnavailableException upstream:
                logger.LogWarning(exception, "Upstream {Upstream} unavailable", upstream.UpstreamName);
                return await WriteProblemAsync(httpContext, exception, StatusCodes.Status503ServiceUnavailable,
                    "Service unavailable", "A data source is temporarily unavailable. Please try again later.");

            default:
                logger.LogError(exception, "Unhandled exception");
                return await WriteProblemAsync(httpContext, exception, StatusCodes.Status500InternalServerError,
                    "Internal server error", "An unexpected error occurred.");
        }
    }

    private ValueTask<bool> WriteProblemAsync(HttpContext httpContext, Exception exception, int status, string title, string detail)
    {
        httpContext.Response.StatusCode = status;
        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Title = title, Detail = detail },
        });
    }
}
