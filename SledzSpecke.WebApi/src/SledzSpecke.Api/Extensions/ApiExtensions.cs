using SledzSpecke.Api.Middleware;
using SledzSpecke.Api.Endpoints;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

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
        
        // Enable static files serving with proper content types
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".md"] = "text/markdown; charset=utf-8";
        
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            OnPrepareResponse = ctx =>
            {
                // Add UTF-8 charset to text files
                if (ctx.Context.Response.ContentType?.StartsWith("text/") == true &&
                    !ctx.Context.Response.ContentType.Contains("charset"))
                {
                    ctx.Context.Response.ContentType += "; charset=utf-8";
                }
            }
        });
        
        // Serve E2E test results if directory exists
        var e2eResultsPath = Path.Combine(app.Environment.ContentRootPath, "e2e-results");
        if (Directory.Exists(e2eResultsPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(e2eResultsPath),
                RequestPath = "/e2e-results",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                }
            });
        }
        
        // Use enhanced middleware pipeline
        app.UseCustomMiddleware();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Map controllers (existing API v1)
        app.MapControllers();
        
        // Map minimal API endpoints (new API v2)
        app.MapMedicalShiftEndpoints();
        app.MapInternshipEndpoints();
        app.MapModuleEndpoints();

        return app;
    }
}