using SledzSpecke.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that wraps command execution in a database transaction
/// Ensures all-or-nothing semantics for complex operations
/// </summary>
internal sealed class TransactionCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionCommandHandlerDecorator<TCommand>> _logger;

    public TransactionCommandHandlerDecorator(
        ICommandHandler<TCommand> handler,
        IUnitOfWork unitOfWork,
        ILogger<TransactionCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogDebug("Starting transaction for command {CommandName}", commandName);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _handler.HandleAsync(command);

            await _unitOfWork.CommitTransactionAsync();
            _logger.LogDebug("Transaction committed for command {CommandName}", commandName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for command {CommandName}, rolling back", commandName);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

/// <summary>
/// Decorator for command handlers that return a result
/// </summary>
internal sealed class TransactionCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionCommandHandlerDecorator<TCommand, TResult>> _logger;

    public TransactionCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> handler,
        IUnitOfWork unitOfWork,
        ILogger<TransactionCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _handler = handler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogDebug("Starting transaction for command {CommandName}", commandName);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await _handler.HandleAsync(command);

            await _unitOfWork.CommitTransactionAsync();
            _logger.LogDebug("Transaction committed for command {CommandName}", commandName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for command {CommandName}, rolling back", commandName);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}