using System.Diagnostics;
using System.Text.Json;

namespace SledzSpecke.Api.Middleware;

/// <summary>
/// Middleware to log SMK-specific operations for audit and troubleshooting
/// </summary>
public class SmkOperationLoggingMiddleware : IMiddleware
{
    private readonly ILogger<SmkOperationLoggingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private readonly HashSet<string> _smkEndpoints = new()
    {
        "/api/export",
        "/api/users/smk-details",
        "/api/specializations/smk-details",
        "/api/additional-self-education-days",
        "/api/test-export"
    };

    public SmkOperationLoggingMiddleware(
        ILogger<SmkOperationLoggingMiddleware> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if this is an SMK-related endpoint
        if (!IsSmkEndpoint(context.Request.Path))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;
        var userId = context.User?.FindFirst("sub")?.Value ?? "Anonymous";

        // Log SMK operation start
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["OperationType"] = "SMK",
            ["UserId"] = userId,
            ["RequestPath"] = context.Request.Path.ToString(),
            ["RequestMethod"] = context.Request.Method
        }))
        {
            _logger.LogInformation(
                "SMK operation started: {Method} {Path} by user {UserId}",
                context.Request.Method,
                context.Request.Path,
                userId);

            try
            {
                await next(context);

                stopwatch.Stop();

                // Log successful SMK operation
                _logger.LogInformation(
                    "SMK operation completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // Log export operations with additional detail
                if (context.Request.Path.StartsWithSegments("/api/export"))
                {
                    LogExportOperation(context, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "SMK operation failed: {Method} {Path} - Duration: {Duration}ms - Error: {ErrorMessage}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }
    }

    private bool IsSmkEndpoint(PathString path)
    {
        return _smkEndpoints.Any(endpoint => path.StartsWithSegments(endpoint));
    }

    private void LogExportOperation(HttpContext context, long duration)
    {
        var pathSegments = context.Request.Path.Value?.Split('/');
        if (pathSegments != null && pathSegments.Length > 4)
        {
            var specializationId = pathSegments[4];
            var exportType = pathSegments.Length > 5 ? pathSegments[5] : "unknown";

            _logger.LogInformation(
                "SMK Export operation details - Type: {ExportType}, SpecializationId: {SpecializationId}, Duration: {Duration}ms, ResponseSize: {ResponseSize} bytes",
                exportType,
                specializationId,
                duration,
                context.Response.ContentLength ?? 0);
        }
    }
}