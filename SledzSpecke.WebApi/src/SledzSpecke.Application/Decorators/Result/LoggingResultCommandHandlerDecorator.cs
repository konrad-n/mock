using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Decorators.Result;

internal sealed class LoggingResultCommandHandlerDecorator<TCommand, TResult> 
    : IResultCommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly IResultCommandHandler<TCommand, TResult> _handler;
    private readonly ILogger<LoggingResultCommandHandlerDecorator<TCommand, TResult>> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new () 
    { 
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public LoggingResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand, TResult> handler,
        ILogger<LoggingResultCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Executing command {CommandName} at {Timestamp}",
            commandName,
            DateTime.UtcNow);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var commandJson = JsonSerializer.Serialize(command, JsonOptions);
            _logger.LogDebug(
                "Command {CommandName} data: {CommandData}",
                commandName,
                commandJson);
        }

        try
        {
            var result = await _handler.HandleAsync(command, cancellationToken);
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Command {CommandName} succeeded in {ElapsedMs}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    "Command {CommandName} failed in {ElapsedMs}ms. Error: {Error}, Code: {ErrorCode}",
                    commandName,
                    stopwatch.ElapsedMilliseconds,
                    result.Error,
                    result.ErrorCode);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Command {CommandName} threw exception after {ElapsedMs}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

// Decorator for commands without return value
internal sealed class LoggingResultCommandHandlerDecorator<TCommand> 
    : IResultCommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly IResultCommandHandler<TCommand> _handler;
    private readonly ILogger<LoggingResultCommandHandlerDecorator<TCommand>> _logger;

    public LoggingResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand> handler,
        ILogger<LoggingResultCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Executing command {CommandName}",
            commandName);

        try
        {
            var result = await _handler.HandleAsync(command, cancellationToken);
            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Command {CommandName} succeeded in {ElapsedMs}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    "Command {CommandName} failed in {ElapsedMs}ms. Error: {Error}",
                    commandName,
                    stopwatch.ElapsedMilliseconds,
                    result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Command {CommandName} threw exception after {ElapsedMs}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}