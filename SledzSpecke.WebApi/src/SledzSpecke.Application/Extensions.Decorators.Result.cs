using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Decorators.Result;

namespace SledzSpecke.Application;

public static partial class ApplicationExtensions
{
    /// <summary>
    /// Registers decorators for Result-based handlers
    /// Order matters: decorators are applied in reverse order of registration
    /// </summary>
    public static IServiceCollection AddResultDecorators(this IServiceCollection services)
    {
        // Check if Scrutor is available
        try
        {
            // Decorate IResultCommandHandler<TCommand, TResult>
            services.Decorate(
                typeof(IResultCommandHandler<,>), 
                typeof(LoggingResultCommandHandlerDecorator<,>));
                
            services.Decorate(
                typeof(IResultCommandHandler<,>), 
                typeof(PerformanceResultCommandHandlerDecorator<,>));
                
            services.Decorate(
                typeof(IResultCommandHandler<,>), 
                typeof(ValidationResultCommandHandlerDecorator<,>));

            // Decorate IResultCommandHandler<TCommand> (no result)
            services.Decorate(
                typeof(IResultCommandHandler<>), 
                typeof(LoggingResultCommandHandlerDecorator<>));
                
            services.Decorate(
                typeof(IResultCommandHandler<>), 
                typeof(PerformanceResultCommandHandlerDecorator<>));
                
            services.Decorate(
                typeof(IResultCommandHandler<>), 
                typeof(ValidationResultCommandHandlerDecorator<>));
        }
        catch
        {
            // If Scrutor is not available, skip decorator registration
            // This allows the application to still work without decorators
        }

        return services;
    }
}