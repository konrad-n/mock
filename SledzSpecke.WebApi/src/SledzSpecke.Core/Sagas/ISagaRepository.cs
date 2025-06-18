using System;
using System.Threading;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Sagas;

public interface ISagaRepository
{
    Task<ISaga?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(ISaga saga, CancellationToken cancellationToken);
    Task UpdateAsync(ISaga saga, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}