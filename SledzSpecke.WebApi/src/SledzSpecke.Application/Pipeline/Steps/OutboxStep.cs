using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Outbox;

namespace SledzSpecke.Application.Pipeline.Steps;

public class OutboxStep : IPipelineStep<MessageContext>
{
    private readonly Core.Outbox.IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxStep> _logger;

    public OutboxStep(Core.Outbox.IOutboxRepository outboxRepository, ILogger<OutboxStep> logger)
    {
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        context.Log($"Persisting message {context.MessageId} to outbox");
        
        // Create outbox message before processing
        var metadata = new Dictionary<string, object>(context.Headers)
        {
            ["MessageId"] = context.MessageId,
            ["CreatedAt"] = context.CreatedAt,
            ["Pipeline"] = true
        };

        var outboxMessage = new OutboxMessage(
            context.MessageType,
            JsonSerializer.Serialize(context.Payload),
            metadata
        );

        try
        {
            // Save to outbox first
            await _outboxRepository.AddAsync(outboxMessage);
            context.Log("Message persisted to outbox");
            
            // Process the message
            await next();
            
            // Mark as processed if successful
            outboxMessage.MarkAsProcessed();
            await _outboxRepository.UpdateAsync(outboxMessage);
            
            context.IsProcessed = true;
            context.Log("Message processed successfully");
            
            _logger.LogInformation("Message {MessageId} processed successfully", context.MessageId);
        }
        catch (Exception ex)
        {
            // Mark as failed in outbox
            outboxMessage.MarkAsFailed(ex.Message);
            await _outboxRepository.UpdateAsync(outboxMessage);
            
            context.Log($"Message processing failed: {ex.Message}");
            _logger.LogError(ex, "Failed to process message {MessageId}", context.MessageId);
            
            throw;
        }
    }
}