using SledzSpecke.Api.Middleware;

namespace SledzSpecke.Api.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds all custom middleware services to the DI container
    /// </summary>
    public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<CorrelationIdMiddleware>();
        services.AddSingleton<RequestResponseLoggingMiddleware>();
        services.AddSingleton<EnhancedExceptionHandlingMiddleware>();
        services.AddSingleton<SmkOperationLoggingMiddleware>();
        
        // Keep the original one for backward compatibility
        services.AddSingleton<ExceptionHandlingMiddleware>();

        return services;
    }

    /// <summary>
    /// Configures the middleware pipeline with proper ordering
    /// </summary>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        // Correlation ID should be first to ensure all logs have it
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Request/Response logging
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        // Exception handling should be early in the pipeline
        app.UseMiddleware<EnhancedExceptionHandlingMiddleware>();

        // SMK operation logging
        app.UseMiddleware<SmkOperationLoggingMiddleware>();

        return app;
    }
}