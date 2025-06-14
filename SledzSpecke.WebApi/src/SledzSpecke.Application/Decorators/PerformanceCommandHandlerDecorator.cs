using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Diagnostics;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that monitors performance of command handlers
/// Logs warnings for slow operations
/// </summary>
internal sealed class PerformanceCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ILogger<PerformanceCommandHandlerDecorator<TCommand>> _logger;
    private const int SlowOperationThresholdMs = 500;

    public PerformanceCommandHandlerDecorator(
        ICommandHandler<TCommand> handler,
        ILogger<PerformanceCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        await _handler.HandleAsync(command);

        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > SlowOperationThresholdMs)
        {
            _logger.LogWarning(
                "Slow operation detected: Command {CommandName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                commandName,
                stopwatch.ElapsedMilliseconds,
                SlowOperationThresholdMs);
        }
    }
}

/// <summary>
/// Decorator for command handlers that return a result
/// </summary>
internal sealed class PerformanceCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly ILogger<PerformanceCommandHandlerDecorator<TCommand, TResult>> _logger;
    private const int SlowOperationThresholdMs = 500;

    public PerformanceCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> handler,
        ILogger<PerformanceCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        var result = await _handler.HandleAsync(command);

        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > SlowOperationThresholdMs)
        {
            _logger.LogWarning(
                "Slow operation detected: Command {CommandName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                commandName,
                stopwatch.ElapsedMilliseconds,
                SlowOperationThresholdMs);
        }

        return result;
    }
}