using SledzSpecke.Api.Middleware;

namespace SledzSpecke.Api.Extensions;

public static class ApiExtensions
{
    /// <summary>
    /// Configure API-specific services
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add custom middleware
        services.AddCustomMiddleware();
        
        // Add other API-specific services here
        
        return services;
    }
    
    /// <summary>
    /// Configure the API middleware pipeline
    /// </summary>
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Override the infrastructure middleware configuration
        // Enable Swagger in both Development and Production for easier testing
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SledzSpecke API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "SledzSpecke API Documentation";
        });

        app.UseHttpsRedirection();
        
        // Enable static files serving
        app.UseStaticFiles();
        
        // Use enhanced middleware pipeline
        app.UseCustomMiddleware();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}