using System.Net;
using System.Text.Json;

namespace Gartenzwerge.API.Middleware;

/// <summary>
/// Catches unhandled exceptions globally and converts them into
/// standardized JSON error responses.
///
/// This prevents leaking internal exception details to API clients
/// and keeps error handling consistent across all controllers.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Creates a new exception middleware instance.
    /// </summary>
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the next middleware and catches unexpected exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing the request.");

            await HandleExceptionAsync(context);
        }
    }

    /// <summary>
    /// Writes a standardized error response.
    /// </summary>
    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected error occurred.",
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}