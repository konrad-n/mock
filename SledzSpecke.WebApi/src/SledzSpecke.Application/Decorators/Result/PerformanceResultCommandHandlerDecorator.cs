using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Decorators.Result;

internal sealed class PerformanceResultCommandHandlerDecorator<TCommand, TResult> 
    : IResultCommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly IResultCommandHandler<TCommand, TResult> _handler;
    private readonly ILogger<PerformanceResultCommandHandlerDecorator<TCommand, TResult>> _logger;
    private const int SlowCommandThresholdMs = 500;

    public PerformanceResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand, TResult> handler,
        ILogger<PerformanceResultCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        
        using var activity = Activity.StartActivity($"Command: {commandName}");
        activity?.SetTag("command.type", commandName);
        
        var stopwatch = Stopwatch.StartNew();
        var result = await _handler.HandleAsync(command, cancellationToken);
        stopwatch.Stop();
        
        activity?.SetTag("command.duration_ms", stopwatch.ElapsedMilliseconds);
        activity?.SetTag("command.success", result.IsSuccess);
        
        if (stopwatch.ElapsedMilliseconds > SlowCommandThresholdMs)
        {
            _logger.LogWarning(
                "Slow command detected: {CommandName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                commandName,
                stopwatch.ElapsedMilliseconds,
                SlowCommandThresholdMs);
        }
        
        return result;
    }
}

internal sealed class PerformanceResultCommandHandlerDecorator<TCommand> 
    : IResultCommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly IResultCommandHandler<TCommand> _handler;
    private readonly ILogger<PerformanceResultCommandHandlerDecorator<TCommand>> _logger;
    private const int SlowCommandThresholdMs = 500;

    public PerformanceResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand> handler,
        ILogger<PerformanceResultCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        
        using var activity = Activity.StartActivity($"Command: {commandName}");
        activity?.SetTag("command.type", commandName);
        
        var stopwatch = Stopwatch.StartNew();
        var result = await _handler.HandleAsync(command, cancellationToken);
        stopwatch.Stop();
        
        activity?.SetTag("command.duration_ms", stopwatch.ElapsedMilliseconds);
        activity?.SetTag("command.success", result.IsSuccess);
        
        if (stopwatch.ElapsedMilliseconds > SlowCommandThresholdMs)
        {
            _logger.LogWarning(
                "Slow command detected: {CommandName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                commandName,
                stopwatch.ElapsedMilliseconds,
                SlowCommandThresholdMs);
        }
        
        return result;
    }
}