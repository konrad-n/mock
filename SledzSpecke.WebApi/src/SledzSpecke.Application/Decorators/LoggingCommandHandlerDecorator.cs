using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Diagnostics;
using System.Text.Json;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that adds logging to command handlers
/// Following MySpot's pattern for cross-cutting concerns
/// </summary>
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
        
        _logger.LogInformation(
            "Executing command {CommandName} with data: {CommandData}",
            commandName,
            JsonSerializer.Serialize(command));

        try
        {
            await _handler.HandleAsync(command);
            
            stopwatch.Stop();
            _logger.LogInformation(
                "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Command {CommandName} failed after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}

/// <summary>
/// Decorator for command handlers that return a result
/// </summary>
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
        
        _logger.LogInformation(
            "Executing command {CommandName} with data: {CommandData}",
            commandName,
            JsonSerializer.Serialize(command));

        try
        {
            var result = await _handler.HandleAsync(command);
            
            stopwatch.Stop();
            _logger.LogInformation(
                "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms with result: {Result}",
                commandName,
                stopwatch.ElapsedMilliseconds,
                JsonSerializer.Serialize(result));
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Command {CommandName} failed after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}