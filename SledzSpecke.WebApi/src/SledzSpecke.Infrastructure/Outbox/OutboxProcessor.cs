using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Outbox;
using SledzSpecke.Core.Repositories;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 50;
    
    public OutboxProcessor(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor> logger)
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
            
            await Task.Delay(_interval, stoppingToken);
        }
        
        _logger.LogInformation("Outbox processor stopped");
    }
    
    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<Core.Outbox.IOutboxRepository>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        var messages = await repository.GetUnprocessedAsync(_batchSize);
        
        if (!messages.Any())
            return;
            
        _logger.LogInformation("Processing {Count} outbox messages", messages.Count());
        
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
            
            await repository.UpdateAsync(message);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Processed {Count} outbox messages", messages.Count());
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