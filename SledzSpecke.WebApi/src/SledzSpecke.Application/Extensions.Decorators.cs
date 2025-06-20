using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Decorators;
using Scrutor;

namespace SledzSpecke.Application;

public static partial class ApplicationExtensions
{
    /// <summary>
    /// Registers decorators for command handlers following the decorator pattern
    /// Order matters: decorators are applied in reverse order of registration
    /// </summary>
    public static IServiceCollection AddDecorators(this IServiceCollection services)
    {
        // Decorate ICommandHandler<TCommand> (void handlers)
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
        services.Decorate(typeof(ICommandHandler<>), typeof(PerformanceCommandHandlerDecorator<>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
        services.Decorate(typeof(ICommandHandler<>), typeof(TransactionCommandHandlerDecorator<>));
        
        // Decorate ICommandHandler<TCommand, TResult> (handlers with results)
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(PerformanceCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionCommandHandlerDecorator<,>));

        // Decorate IQueryHandler<TQuery, TResult>
        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(PerformanceQueryHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(CachingQueryHandlerDecorator<,>));

        // If you have IResultCommandHandler decorators, add them here too
        // services.Decorate(typeof(IResultCommandHandler<>), typeof(LoggingResultCommandHandlerDecorator<>));
        // services.Decorate(typeof(IResultCommandHandler<,>), typeof(LoggingResultCommandHandlerDecorator<,>));

        return services;
    }
    
    /// <summary>
    /// Alternative method to decorate specific handlers only
    /// </summary>
    public static IServiceCollection AddSelectiveDecorators(this IServiceCollection services)
    {
        // Example: Only decorate specific critical handlers
        // Medical shift decorators are now handled by Result-based decorators
        
        return services;
    }
}