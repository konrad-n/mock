using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Pipeline;
using SledzSpecke.Application.Pipeline.Steps;

namespace SledzSpecke.Application.Extensions;

public static class MessagePipelineExtensions
{
    public static IServiceCollection AddMessagePipeline(this IServiceCollection services)
    {
        // Register the message executor
        services.AddScoped<IMessageExecutor, MessageExecutor>();
        
        // Register execution steps
        services.AddScoped<IMessageExecutionStep, ValidationExecutionStep>();
        services.AddScoped<IMessageExecutionStep, LoggingExecutionStep>();
        services.AddScoped<IMessageExecutionStep, TimingExecutionStep>();
        services.AddScoped<IMessageExecutionStep, OutboxExecutionStep>();
        
        return services;
    }
}