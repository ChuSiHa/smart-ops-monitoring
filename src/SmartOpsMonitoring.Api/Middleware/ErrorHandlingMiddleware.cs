namespace SmartOpsMonitoring.Api.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and returns a standardised
/// <see cref="ProblemDetails"/> JSON response.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="ErrorHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware, forwarding the request to the next handler and catching exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
        }
        catch (Exception ex)
        {
            var safeMethod = Sanitize(context.Request.Method);
            var safePath = Sanitize(context.Request.Path);
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", safeMethod, safePath);
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Internal Server Error",
                "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Sanitizes a string value by replacing carriage-return and line-feed characters to
    /// prevent log-injection attacks.
    /// </summary>
    /// <param name="value">The value to sanitize.</param>
    /// <returns>The sanitized string.</returns>
    private static string Sanitize(string? value)
        => (value ?? string.Empty).Replace("\r", "\\r", StringComparison.Ordinal)
                                   .Replace("\n", "\\n", StringComparison.Ordinal);

    /// <summary>
    /// Writes a <see cref="ProblemDetails"/> JSON response to the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="statusCode">The HTTP status code to set.</param>
    /// <param name="title">A short, human-readable summary of the problem.</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
