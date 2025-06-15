using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Decorators;
using Scrutor;

namespace SledzSpecke.Application;

public static partial class Extensions
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
        services.Decorate<ICommandHandler<Commands.AddMedicalShift, int>, LoggingCommandHandlerDecorator<Commands.AddMedicalShift, int>>();
        services.Decorate<ICommandHandler<Commands.AddMedicalShift, int>, PerformanceCommandHandlerDecorator<Commands.AddMedicalShift, int>>();
        
        return services;
    }
}