using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using MediatR;

namespace SledzSpecke.Infrastructure.Events;

public class EventPublisher : IEventPublisher
{
    private readonly IPublisher _mediator;
    private readonly ILogger<EventPublisher> _logger;
    
    public EventPublisher(IPublisher mediator, ILogger<EventPublisher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class
    {
        _logger.LogDebug("Publishing event {EventType}", @event.GetType().Name);
        
        // If the event implements INotification, publish via MediatR
        if (@event is INotification notification)
        {
            await _mediator.Publish(notification, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Event {EventType} does not implement INotification and cannot be published", @event.GetType().Name);
        }
    }
}