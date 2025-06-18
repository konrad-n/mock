using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Services;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Pipeline.Steps;

public sealed class OutboxExecutionStep : IMessageExecutionStep
{
    private readonly IOutboxService _outboxService;
    private readonly ILogger<OutboxExecutionStep> _logger;
    
    public OutboxExecutionStep(IOutboxService outboxService, ILogger<OutboxExecutionStep> logger)
    {
        _outboxService = outboxService;
        _logger = logger;
    }
    
    public async Task ExecuteAsync<TMessage>(
        TMessage message, 
        Func<Task> next, 
        CancellationToken cancellationToken = default) where TMessage : class
    {
        // Execute handler first
        await next();
        
        // Then store events in outbox if the message implements IHasEvents
        if (message is IHasEvents hasEvents)
        {
            var events = hasEvents.GetEvents();
            foreach (var @event in events)
            {
                var messageId = await _outboxService.StoreAsync(@event);
                
                _logger.LogDebug(
                    "Stored event {EventType} in outbox with ID {MessageId}",
                    @event.GetType().Name,
                    messageId
                );
            }
        }
    }
}