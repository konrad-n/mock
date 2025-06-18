using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Outbox;
using System.Text.Json;
using System.Diagnostics;

namespace SledzSpecke.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);
    
    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }
            
            await Task.Delay(_checkInterval, stoppingToken);
        }
        
        _logger.LogInformation("Outbox processor stopped");
    }
    
    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        
        var messages = await repository.GetUnprocessedAsync(50, cancellationToken);
        
        if (messages.Count == 0)
            return;
            
        _logger.LogInformation("Processing {Count} outbox messages", messages.Count);
        
        foreach (var message in messages)
        {
            try
            {
                await ProcessMessage(message, eventPublisher, cancellationToken);
                message.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Failed to process outbox message {MessageId} of type {Type}",
                    message.Id,
                    message.Type
                );
                message.MarkAsFailed(ex.Message);
            }
            
            await repository.UpdateAsync(message, cancellationToken);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Processed {Count} outbox messages", messages.Count);
    }
    
    private async Task ProcessMessage(
        OutboxMessage message, 
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        var eventType = Type.GetType(message.Type);
        if (eventType is null)
        {
            throw new InvalidOperationException($"Cannot resolve type: {message.Type}");
        }
        
        var @event = JsonSerializer.Deserialize(message.Data, eventType);
        if (@event is null)
        {
            throw new InvalidOperationException($"Cannot deserialize message data");
        }
        
        await eventPublisher.PublishAsync(@event, cancellationToken);
    }
}