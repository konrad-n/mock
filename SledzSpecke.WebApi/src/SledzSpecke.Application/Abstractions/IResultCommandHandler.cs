using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Command handler that returns a Result (no value)
/// </summary>
public interface IResultCommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Command handler that returns a Result with value
/// </summary>
public interface IResultCommandHandler<in TCommand, TResult> where TCommand : class, ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}