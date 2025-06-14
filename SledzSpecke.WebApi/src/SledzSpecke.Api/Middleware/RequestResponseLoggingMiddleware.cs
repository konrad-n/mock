using System.Diagnostics;
using System.Text;

namespace SledzSpecke.Api.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with performance metrics
/// </summary>
public class RequestResponseLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key"
    };

    public RequestResponseLoggingMiddleware(
        ILogger<RequestResponseLoggingMiddleware> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip logging for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);

        // Log request
        LogRequest(context, requestBody);

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            LogResponse(context, responseBodyText, stopwatch.ElapsedMilliseconds);

            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 512,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private void LogRequest(HttpContext context, string requestBody)
    {
        var request = context.Request;
        var logLevel = _environment.IsDevelopment() ? LogLevel.Information : LogLevel.Debug;

        var requestLog = new StringBuilder();
        requestLog.AppendLine($"HTTP Request Information:");
        requestLog.AppendLine($"Schema: {request.Scheme}");
        requestLog.AppendLine($"Host: {request.Host}");
        requestLog.AppendLine($"Path: {request.Path}");
        requestLog.AppendLine($"QueryString: {request.QueryString}");
        requestLog.AppendLine($"Headers: {GetSafeHeaders(request.Headers)}");

        if (!string.IsNullOrWhiteSpace(requestBody) && ShouldLogBody(request.ContentType))
        {
            requestLog.AppendLine($"Body: {TruncateBody(requestBody)}");
        }

        _logger.Log(logLevel, requestLog.ToString());
    }

    private void LogResponse(HttpContext context, string responseBody, long elapsedMs)
    {
        var response = context.Response;
        var logLevel = GetResponseLogLevel(response.StatusCode);

        var responseLog = new StringBuilder();
        responseLog.AppendLine($"HTTP Response Information:");
        responseLog.AppendLine($"StatusCode: {response.StatusCode}");
        responseLog.AppendLine($"Headers: {GetSafeHeaders(response.Headers)}");
        responseLog.AppendLine($"ElapsedTime: {elapsedMs}ms");

        if (!string.IsNullOrWhiteSpace(responseBody) && ShouldLogBody(response.ContentType))
        {
            responseLog.AppendLine($"Body: {TruncateBody(responseBody)}");
        }

        // Add performance warning for slow requests
        if (elapsedMs > 1000)
        {
            responseLog.AppendLine($"⚠️ SLOW REQUEST: {elapsedMs}ms exceeds 1000ms threshold");
        }

        _logger.Log(logLevel, responseLog.ToString());
    }

    private string GetSafeHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = headers
            .Where(h => !SensitiveHeaders.Contains(h.Key))
            .Select(h => $"{h.Key}: {string.Join(", ", h.Value.ToArray())}");

        return string.Join("; ", safeHeaders.ToArray());
    }

    private bool ShouldLogBody(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        // Only log JSON and text content
        return contentType.Contains("json", StringComparison.OrdinalIgnoreCase) ||
               contentType.Contains("text", StringComparison.OrdinalIgnoreCase);
    }

    private string TruncateBody(string body)
    {
        const int maxLength = 4096; // 4KB limit for logging
        
        if (body.Length <= maxLength)
            return body;

        return $"{body.Substring(0, maxLength)}... (truncated)";
    }

    private LogLevel GetResponseLogLevel(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => _environment.IsDevelopment() ? LogLevel.Information : LogLevel.Debug
        };
    }
}