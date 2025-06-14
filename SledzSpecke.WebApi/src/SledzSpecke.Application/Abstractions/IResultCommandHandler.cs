using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Abstractions;

public interface IResultCommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task<Result> HandleAsync(TCommand command);
}

public interface IResultCommandHandler<in TCommand, TResult> where TCommand : class, ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command);
}