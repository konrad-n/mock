using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Outbox;

namespace SledzSpecke.Application.Services;

public sealed class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _repository;
    private readonly ILogger<OutboxService> _logger;
    
    public OutboxService(IOutboxRepository repository, ILogger<OutboxService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Guid> StoreAsync<TEvent>(TEvent @event, Dictionary<string, object>? metadata = null) 
        where TEvent : class
    {
        var eventType = @event.GetType().FullName 
            ?? throw new InvalidOperationException("Cannot determine event type");
            
        var eventData = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        
        var outboxMessage = new OutboxMessage(eventType, eventData, metadata);
        
        await _repository.AddAsync(outboxMessage);
        
        _logger.LogDebug(
            "Stored event {EventType} with ID {MessageId} in outbox",
            eventType,
            outboxMessage.Id
        );
        
        return outboxMessage.Id;
    }
}