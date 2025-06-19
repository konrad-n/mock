using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Adapters;

/// <summary>
/// Adapter that bridges IResultCommandHandler to ICommandHandler
/// </summary>
public class ResultToCommandHandlerAdapter<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly IResultCommandHandler<TCommand> _resultHandler;

    public ResultToCommandHandlerAdapter(IResultCommandHandler<TCommand> resultHandler)
    {
        _resultHandler = resultHandler;
    }

    public async Task HandleAsync(TCommand command)
    {
        var result = await _resultHandler.HandleAsync(command);
        
        if (result.IsFailure)
        {
            throw new InvalidOperationException($"{result.Error} (Code: {result.ErrorCode})");
        }
    }
}

/// <summary>
/// Adapter that bridges IResultCommandHandler with result to ICommandHandler with result
/// </summary>
public class ResultToCommandHandlerAdapter<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly IResultCommandHandler<TCommand, TResult> _resultHandler;

    public ResultToCommandHandlerAdapter(IResultCommandHandler<TCommand, TResult> resultHandler)
    {
        _resultHandler = resultHandler;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        var result = await _resultHandler.HandleAsync(command);
        
        if (result.IsFailure)
        {
            throw new InvalidOperationException($"{result.Error} (Code: {result.ErrorCode})");
        }

        return result.Value;
    }
}