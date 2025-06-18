using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Pipeline.Steps;

public class ValidationStep : IPipelineStep<MessageContext>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidationStep> _logger;

    public ValidationStep(IServiceProvider serviceProvider, ILogger<ValidationStep> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        context.Log($"Validating message {context.MessageId} of type {context.MessageType}");
        
        // Try to find validator for the payload type
        var payloadType = context.Payload.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(payloadType);
        
        var validator = _serviceProvider.GetService(validatorType);
        if (validator == null)
        {
            context.Log($"No validator found for type {payloadType.Name}, skipping validation");
            await next();
            return;
        }

        try
        {
            // Use reflection to invoke Validate method
            var validateMethod = validatorType.GetMethod("Validate");
            var result = (Result)validateMethod!.Invoke(validator, new[] { context.Payload })!;
            
            if (!result.IsSuccess)
            {
                context.ErrorMessage = result.Error;
                context.Log($"Validation failed: {context.ErrorMessage}");
                _logger.LogWarning("Validation failed for message {MessageId}: {Error}", 
                    context.MessageId, context.ErrorMessage);
                return; // Short-circuit pipeline
            }
            
            context.Log("Validation passed");
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during validation for message {MessageId}", context.MessageId);
            context.ErrorMessage = $"Validation error: {ex.Message}";
            context.Log($"Validation error: {ex.Message}");
            // Don't continue pipeline on validation error
        }
    }
}