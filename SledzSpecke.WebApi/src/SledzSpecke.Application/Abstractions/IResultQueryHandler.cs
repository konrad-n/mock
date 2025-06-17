using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Query handler that returns a Result with value
/// </summary>
public interface IResultQueryHandler<in TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}