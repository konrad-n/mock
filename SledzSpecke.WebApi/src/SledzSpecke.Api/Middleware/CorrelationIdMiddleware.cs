namespace SledzSpecke.Api.Middleware;

/// <summary>
/// Middleware to handle correlation IDs for request tracking across services
/// </summary>
public class CorrelationIdMiddleware : IMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Add to response headers
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);
        
        // Add to logging context
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path.ToString(),
            ["RequestMethod"] = context.Request.Method
        }))
        {
            _logger.LogInformation("Request started: {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            await next(context);
            
            _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode}", 
                context.Request.Method, context.Request.Path, context.Response.StatusCode);
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Try to get from request header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) && 
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId!;
        }

        // Generate new correlation ID
        var newCorrelationId = Guid.NewGuid().ToString("N");
        
        // Store in HttpContext for access in other parts of the application
        context.Items["CorrelationId"] = newCorrelationId;
        
        return newCorrelationId;
    }
}