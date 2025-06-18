using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Outbox;

namespace SledzSpecke.Application.Pipeline.Steps;

public interface IDeadLetterService
{
    Task MoveToDeadLetterAsync(MessageContext context);
}

public class DeadLetterService : IDeadLetterService
{
    private readonly Core.Outbox.IOutboxRepository _outboxRepository;
    private readonly ILogger<DeadLetterService> _logger;

    public DeadLetterService(Core.Outbox.IOutboxRepository outboxRepository, ILogger<DeadLetterService> logger)
    {
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task MoveToDeadLetterAsync(MessageContext context)
    {
        var deadLetterData = new
        {
            OriginalMessageId = context.MessageId,
            MessageType = context.MessageType,
            Payload = context.Payload,
            Headers = context.Headers,
            CreatedAt = context.CreatedAt,
            FailedAt = DateTime.UtcNow,
            RetryCount = context.RetryCount,
            ErrorMessage = context.ErrorMessage,
            ExecutionLog = context.ExecutionLog
        };

        var metadata = new Dictionary<string, object>
        {
            ["IsDeadLetter"] = true,
            ["OriginalMessageType"] = context.MessageType,
            ["FailureReason"] = context.ErrorMessage ?? "Unknown"
        };

        var deadLetterMessage = new OutboxMessage(
            "DeadLetter." + context.MessageType,
            JsonSerializer.Serialize(deadLetterData),
            metadata
        );

        await _outboxRepository.AddAsync(deadLetterMessage);
        
        _logger.LogWarning("Message {MessageId} moved to dead letter queue after {RetryCount} retries", 
            context.MessageId, context.RetryCount);
    }
}

public class DeadLetterStep : IPipelineStep<MessageContext>
{
    private readonly IDeadLetterService _deadLetterService;
    private readonly ILogger<DeadLetterStep> _logger;
    private readonly int _maxRetries;

    public DeadLetterStep(IDeadLetterService deadLetterService, ILogger<DeadLetterStep> logger, int maxRetries = 3)
    {
        _deadLetterService = deadLetterService;
        _logger = logger;
        _maxRetries = maxRetries;
    }

    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message {MessageId}", context.MessageId);
            
            if (context.RetryCount >= _maxRetries)
            {
                context.Log($"Moving to dead letter after {context.RetryCount} retries: {ex.Message}");
                context.ErrorMessage = ex.Message;
                
                try
                {
                    await _deadLetterService.MoveToDeadLetterAsync(context);
                    context.Log("Successfully moved to dead letter queue");
                }
                catch (Exception dlEx)
                {
                    _logger.LogError(dlEx, "Failed to move message {MessageId} to dead letter queue", context.MessageId);
                    throw;
                }
                
                // Don't rethrow - message is handled by dead letter
                return;
            }
            
            // Rethrow for retry
            throw;
        }
    }
}