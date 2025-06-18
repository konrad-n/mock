using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Pipeline.Steps;

namespace SledzSpecke.Application.Pipeline;

public interface IPipelineFactory
{
    Func<MessageContext, Task> CreatePipeline(string messageType);
}

public class PipelineFactory : IPipelineFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PipelineFactory> _logger;
    private readonly Dictionary<string, Func<IPipelineBuilder<MessageContext>, IPipelineBuilder<MessageContext>>> _configurations = new();

    public PipelineFactory(IServiceProvider serviceProvider, ILogger<PipelineFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        RegisterPipelines();
    }

    private void RegisterPipelines()
    {
        // Medical shift pipeline - critical for SMK compliance
        _configurations["MedicalShift"] = builder => builder
            .UseStep<ValidationStep>()
            .UseStep<DeadLetterStep>()
            .UseStep<RetryStep>()
            .Use(async (context, next) =>
            {
                // Custom medical shift logic - validate SMK requirements
                context.Log("Processing medical shift with SMK validation");
                
                // Check weekly hours limit (48 hours max)
                if (context.Headers.TryGetValue("WeeklyHours", out var weeklyHours) && 
                    Convert.ToInt32(weeklyHours) > 48)
                {
                    context.ErrorMessage = "Przekroczono tygodniowy limit 48 godzin dyżurów";
                    context.Log($"Weekly hours validation failed: {weeklyHours} > 48");
                    return;
                }
                
                await next();
            })
            .UseStep<OutboxStep>();

        // Procedure pipeline - for medical procedures tracking
        _configurations["Procedure"] = builder => builder
            .UseStep<ValidationStep>()
            .UseWhen(ctx => ctx.Headers.ContainsKey("Priority") && ctx.Headers["Priority"].Equals("High"),
                async (context, next) =>
                {
                    context.Log("High priority procedure processing");
                    // Priority processing logic
                    await next();
                })
            .UseStep<DeadLetterStep>()
            .UseStep<RetryStep>()
            .UseStep<OutboxStep>();

        // Internship pipeline - for internship management
        _configurations["Internship"] = builder => builder
            .UseStep<ValidationStep>()
            .Use(async (context, next) =>
            {
                context.Log("Processing internship message");
                
                // Check monthly hours minimum (160 hours)
                if (context.Headers.TryGetValue("MonthlyHours", out var monthlyHours) && 
                    Convert.ToInt32(monthlyHours) < 160)
                {
                    context.Headers["Warning"] = "Nie osiągnięto minimalnej liczby 160 godzin miesięcznie";
                    context.Log($"Monthly hours warning: {monthlyHours} < 160");
                }
                
                await next();
            })
            .UseStep<OutboxStep>();

        // SMK Report pipeline - for monthly report generation
        _configurations["SMKReport"] = builder => builder
            .UseStep<ValidationStep>()
            .UseStep<DeadLetterStep>()
            .UseStep<RetryStep>()
            .Use(async (context, next) =>
            {
                context.Log("Generating SMK monthly report");
                // Add report generation timestamp
                context.Headers["ReportGeneratedAt"] = DateTime.UtcNow.ToString("O");
                await next();
            })
            .UseStep<OutboxStep>();

        // User registration pipeline
        _configurations["UserRegistration"] = builder => builder
            .UseStep<ValidationStep>()
            .Use(async (context, next) =>
            {
                context.Log("Processing user registration");
                // Add registration timestamp
                context.Headers["RegisteredAt"] = DateTime.UtcNow.ToString("O");
                await next();
            })
            .UseStep<OutboxStep>();

        // Module completion pipeline
        _configurations["ModuleCompletion"] = builder => builder
            .UseStep<ValidationStep>()
            .UseStep<DeadLetterStep>()
            .UseStep<RetryStep>()
            .Use(async (context, next) =>
            {
                context.Log("Processing module completion");
                // Track module progression
                if (context.Headers.TryGetValue("ModuleType", out var moduleType))
                {
                    context.Log($"Completing module: {moduleType}");
                }
                await next();
            })
            .UseStep<OutboxStep>();
    }

    public Func<MessageContext, Task> CreatePipeline(string messageType)
    {
        var builder = new PipelineBuilder<MessageContext>(_serviceProvider);
        
        if (_configurations.TryGetValue(messageType, out var configure))
        {
            _logger.LogInformation("Creating specialized pipeline for message type: {MessageType}", messageType);
            configure(builder);
        }
        else
        {
            _logger.LogInformation("Creating default pipeline for message type: {MessageType}", messageType);
            // Default pipeline for unknown message types
            builder
                .UseStep<ValidationStep>()
                .UseStep<DeadLetterStep>()
                .UseStep<RetryStep>()
                .UseStep<OutboxStep>();
        }
        
        return builder.Build();
    }
}