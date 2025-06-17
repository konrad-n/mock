using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SledzSpecke.Application.Pipeline;

namespace SledzSpecke.Application.Pipeline.Steps;

public sealed class TimingExecutionStep : IMessageExecutionStep
{
    private readonly ILogger<TimingExecutionStep> _logger;

    public TimingExecutionStep(ILogger<TimingExecutionStep> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync<TMessage>(TMessage message, Func<Task> next, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var messageName = typeof(TMessage).Name;
        var stopwatch = Stopwatch.StartNew();

        await next();

        stopwatch.Stop();
        _logger.LogInformation("{MessageType} executed in {ElapsedMilliseconds}ms", 
            messageName, stopwatch.ElapsedMilliseconds);
    }
}