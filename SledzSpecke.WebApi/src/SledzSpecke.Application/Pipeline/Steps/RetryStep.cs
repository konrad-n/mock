using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace SledzSpecke.Application.Pipeline.Steps;

public interface IRetryPolicy
{
    Task ExecuteAsync(Func<Task> action);
}

public class DefaultRetryPolicy : IRetryPolicy
{
    private readonly AsyncRetryPolicy _policy;
    private readonly ILogger<DefaultRetryPolicy> _logger;

    public DefaultRetryPolicy(ILogger<DefaultRetryPolicy> logger)
    {
        _logger = logger;
        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount} after {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await _policy.ExecuteAsync(action);
    }
}

public class RetryStep : IPipelineStep<MessageContext>
{
    private readonly IRetryPolicy _retryPolicy;
    private readonly ILogger<RetryStep> _logger;

    public RetryStep(IRetryPolicy retryPolicy, ILogger<RetryStep> logger)
    {
        _retryPolicy = retryPolicy;
        _logger = logger;
    }

    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        context.Log($"Executing with retry policy for message {context.MessageId}");
        
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.RetryCount++;
                    context.Log($"Retry {context.RetryCount} failed: {ex.Message}");
                    _logger.LogWarning(ex, "Retry {RetryCount} failed for message {MessageId}", 
                        context.RetryCount, context.MessageId);
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            // Final failure after all retries
            context.ErrorMessage = $"Failed after {context.RetryCount} retries: {ex.Message}";
            context.Log($"Final failure: {ex.Message}");
            _logger.LogError(ex, "Message {MessageId} failed after {RetryCount} retries", 
                context.MessageId, context.RetryCount);
            throw;
        }
    }
}