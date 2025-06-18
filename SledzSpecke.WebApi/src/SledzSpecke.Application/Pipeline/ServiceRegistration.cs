using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Pipeline.Steps;

namespace SledzSpecke.Application.Pipeline;

public static class PipelineServiceRegistration
{
    public static IServiceCollection AddEnhancedMessagePipeline(this IServiceCollection services)
    {
        // Register pipeline infrastructure
        services.AddSingleton<IPipelineFactory, PipelineFactory>();
        
        // Register pipeline steps
        services.AddScoped<ValidationStep>();
        services.AddScoped<RetryStep>();
        services.AddScoped<DeadLetterStep>();
        services.AddScoped<OutboxStep>();
        
        // Register retry policy
        services.AddSingleton<IRetryPolicy, DefaultRetryPolicy>();
        
        // Register dead letter service
        services.AddScoped<IDeadLetterService, DeadLetterService>();
        
        return services;
    }
}