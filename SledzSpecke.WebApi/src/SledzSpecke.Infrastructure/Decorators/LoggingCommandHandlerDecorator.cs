using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Diagnostics;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Decorators;

internal sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;

    public LoggingCommandHandlerDecorator(
        ICommandHandler<TCommand> handler,
        ILogger<LoggingCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling command {CommandName}", commandName);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var commandJson = JsonSerializer.Serialize(command);
            _logger.LogDebug("Command details: {CommandJson}", commandJson);
        }

        try
        {
            await _handler.HandleAsync(command);

            stopwatch.Stop();
            _logger.LogInformation(
                "Command {CommandName} handled successfully in {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Error handling command {CommandName} after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

internal sealed class LoggingCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> _logger;

    public LoggingCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> handler,
        ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling command {CommandName}", commandName);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var commandJson = JsonSerializer.Serialize(command);
            _logger.LogDebug("Command details: {CommandJson}", commandJson);
        }

        try
        {
            var result = await _handler.HandleAsync(command);

            stopwatch.Stop();
            _logger.LogInformation(
                "Command {CommandName} handled successfully in {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Error handling command {CommandName} after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}