using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Pipeline.Steps;

namespace SledzSpecke.Application.Pipeline.Examples;

/// <summary>
/// Example showing how to use the Enhanced Message Pipeline in practice
/// This demonstrates various pipeline configurations and usage patterns
/// </summary>
public static class PipelineUsageExample
{
    /// <summary>
    /// Example 1: Basic command processing through pipeline
    /// </summary>
    public static async Task ProcessCommandExample(IServiceProvider serviceProvider)
    {
        var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();
        var logger = serviceProvider.GetRequiredService<ILogger<MessageContext>>();

        // Create a message context for a command
        var messageContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "AddMedicalShift",
            Payload = new
            {
                InternshipId = 123,
                Date = DateTime.Today,
                Hours = 8,
                Minutes = 0,
                IsNightShift = false
            },
            CreatedAt = DateTime.UtcNow,
            Headers = new Dictionary<string, object>
            {
                ["UserId"] = "user123",
                ["CorrelationId"] = Guid.NewGuid().ToString()
            }
        };

        // Create pipeline with command configuration
        var pipeline = pipelineFactory.CreatePipeline("command");

        // Process the message
        await pipeline(messageContext);

        // Check results
        if (messageContext.IsProcessed)
        {
            logger.LogInformation("Command processed successfully");
        }
        else
        {
            logger.LogError("Command processing failed: {Error}", messageContext.ErrorMessage);
        }
    }

    /// <summary>
    /// Example 2: Event processing with retry and dead letter
    /// </summary>
    public static async Task ProcessEventExample(IServiceProvider serviceProvider)
    {
        var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();

        // Create event message
        var eventContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "MedicalShiftCreated",
            Payload = new
            {
                ShiftId = 456,
                InternshipId = 123,
                CreatedAt = DateTime.UtcNow
            },
            CreatedAt = DateTime.UtcNow
        };

        // Use event pipeline which includes retry and dead letter handling
        var pipeline = pipelineFactory.CreatePipeline("event");

        await pipeline(eventContext);
    }

    /// <summary>
    /// Example 3: Custom pipeline configuration
    /// </summary>
    public static async Task CustomPipelineExample(IServiceProvider serviceProvider)
    {
        var pipelineBuilder = new PipelineBuilder<MessageContext>(serviceProvider);

        // Build custom pipeline
        var customPipeline = pipelineBuilder
            .Use(async (context, next) =>
            {
                // Custom pre-processing
                context.Log("Starting custom processing");
                await next();
                context.Log("Completed custom processing");
            })
            .Use(async (context, next) =>
            {
                // Add correlation ID if missing
                if (!context.Headers.ContainsKey("CorrelationId"))
                {
                    context.Headers["CorrelationId"] = Guid.NewGuid().ToString();
                }
                await next();
            })
            .UseStep<ValidationStep>()
            .UseStep<RetryStep>()
            .Build();

        // Use the custom pipeline
        var messageContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "CustomMessage",
            Payload = new { Data = "Test" },
            CreatedAt = DateTime.UtcNow
        };

        await customPipeline(messageContext);
    }

    /// <summary>
    /// Example 4: Conditional pipeline steps
    /// </summary>
    public static async Task ConditionalPipelineExample(IServiceProvider serviceProvider)
    {
        var pipelineBuilder = new PipelineBuilder<MessageContext>(serviceProvider);
        var validationStep = serviceProvider.GetRequiredService<ValidationStep>();
        var outboxStep = serviceProvider.GetRequiredService<OutboxStep>();

        var conditionalPipeline = pipelineBuilder
            // Only validate if it's a command
            .UseWhen(
                context => context.MessageType.EndsWith("Command"), 
                async (context, next) =>
                {
                    await validationStep.ExecuteAsync(context, next);
                })
            // Only persist to outbox if it's an event
            .UseWhen(
                context => context.MessageType.EndsWith("Event"),
                async (context, next) =>
                {
                    await outboxStep.ExecuteAsync(context, next);
                })
            .Build();

        // Test with command
        var commandContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "TestCommand",
            Payload = new { },
            CreatedAt = DateTime.UtcNow
        };

        await conditionalPipeline(commandContext);

        // Test with event
        var eventContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "TestEvent",
            Payload = new { },
            CreatedAt = DateTime.UtcNow
        };

        await conditionalPipeline(eventContext);
    }

    /// <summary>
    /// Example 5: Error handling and logging
    /// </summary>
    public static async Task ErrorHandlingExample(IServiceProvider serviceProvider)
    {
        var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();
        var logger = serviceProvider.GetRequiredService<ILogger<MessageContext>>();

        var messageContext = new MessageContext
        {
            MessageId = Guid.NewGuid(),
            MessageType = "FailingCommand",
            Payload = new { WillFail = true },
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var pipeline = pipelineFactory.CreatePipeline("command");
            await pipeline(messageContext);

            // Check execution log
            foreach (var log in messageContext.ExecutionLog)
            {
                logger.LogInformation("Pipeline log: {Log}", log);
            }

            if (!string.IsNullOrEmpty(messageContext.ErrorMessage))
            {
                logger.LogError("Pipeline error: {Error}", messageContext.ErrorMessage);
                // Message might be in dead letter queue now
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in pipeline");
        }
    }
}