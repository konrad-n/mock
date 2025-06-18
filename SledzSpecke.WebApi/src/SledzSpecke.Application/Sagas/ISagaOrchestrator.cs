using System;
using System.Threading;
using System.Threading.Tasks;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Sagas;

namespace SledzSpecke.Application.Sagas;

public interface ISagaOrchestrator<TSaga, TData> 
    where TSaga : SagaBase<TData> 
    where TData : class
{
    Task<Result<Guid>> StartAsync(TData data, CancellationToken cancellationToken);
    Task<Result> ResumeAsync(Guid sagaId, CancellationToken cancellationToken);
    Task<SagaState> GetStateAsync(Guid sagaId, CancellationToken cancellationToken);
}