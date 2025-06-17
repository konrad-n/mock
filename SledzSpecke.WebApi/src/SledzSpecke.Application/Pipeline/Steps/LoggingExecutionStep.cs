using Microsoft.Extensions.Logging;
using System.Text.Json;
using SledzSpecke.Application.Pipeline;

namespace SledzSpecke.Application.Pipeline.Steps;

public sealed class LoggingExecutionStep : IMessageExecutionStep
{
    private readonly ILogger<LoggingExecutionStep> _logger;

    public LoggingExecutionStep(ILogger<LoggingExecutionStep> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync<TMessage>(TMessage message, Func<Task> next, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var messageName = typeof(TMessage).Name;
        var messageJson = JsonSerializer.Serialize(message);

        _logger.LogInformation("Executing {MessageType}: {MessageContent}", messageName, messageJson);

        try
        {
            await next();
            _logger.LogInformation("Successfully executed {MessageType}", messageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute {MessageType}", messageName);
            throw;
        }
    }
}