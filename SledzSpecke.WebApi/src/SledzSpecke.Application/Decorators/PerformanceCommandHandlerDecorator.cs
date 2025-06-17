using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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
    private static readonly Meter _meter = new("SledzSpecke.Application", "1.0");
    private static readonly Counter<long> _commandCounter = _meter.CreateCounter<long>("commands_executed", "commands", "Number of commands executed");
    private static readonly Histogram<double> _commandDuration = _meter.CreateHistogram<double>("command_duration", "ms", "Command execution duration");
    private static readonly Counter<long> _commandErrors = _meter.CreateCounter<long>("command_errors", "errors", "Number of command execution errors");

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
        var tags = new TagList
        {
            { "command", commandName },
            { "has_result", "false" }
        };

        try
        {
            await _handler.HandleAsync(command);

            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            // Record metrics
            _commandCounter.Add(1, tags);
            _commandDuration.Record(duration, tags);

            if (duration > SlowOperationThresholdMs)
            {
                _logger.LogWarning(
                    "Slow operation detected: Command {CommandName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                    commandName,
                    duration,
                    SlowOperationThresholdMs);
            }
        }
        catch (Exception)
        {
            stopwatch.Stop();
            
            // Record error metrics
            _commandErrors.Add(1, tags);
            _commandDuration.Record(stopwatch.ElapsedMilliseconds, tags);

            throw;
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
    private static readonly Meter _meter = new("SledzSpecke.Application", "1.0");
    private static readonly Counter<long> _commandCounter = _meter.CreateCounter<long>("commands_executed", "commands", "Number of commands executed");
    private static readonly Histogram<double> _commandDuration = _meter.CreateHistogram<double>("command_duration", "ms", "Command execution duration");
    private static readonly Counter<long> _commandErrors = _meter.CreateCounter<long>("command_errors", "errors", "Number of command execution errors");

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
        var tags = new TagList
        {
            { "command", commandName },
            { "has_result", "true" }
        };

        try
        {
            var result = await _handler.HandleAsync(command);

            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            // Record metrics
            _commandCounter.Add(1, tags);
            _commandDuration.Record(duration, tags);

            if (duration > SlowOperationThresholdMs)
            {
                _logger.LogWarning(
                    "Slow operation detected: Command {CommandName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                    commandName,
                    duration,
                    SlowOperationThresholdMs);
            }

            return result;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            
            // Record error metrics
            _commandErrors.Add(1, tags);
            _commandDuration.Record(stopwatch.ElapsedMilliseconds, tags);

            throw;
        }
    }
}