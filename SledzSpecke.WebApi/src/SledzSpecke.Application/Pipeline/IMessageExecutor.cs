using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Pipeline;

public interface IMessageExecutor
{
    Task ExecuteCommandAsync<TCommand>(TCommand command)
        where TCommand : class, ICommand;
        
    Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command)
        where TCommand : class, ICommand<TResult>;
        
    Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>;
}