using SledzSpecke.Core.Outbox;

namespace SledzSpecke.Core.Repositories;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message);
    Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100);
    Task UpdateAsync(OutboxMessage message);
    Task<int> GetUnprocessedCountAsync();
    Task DeleteOldProcessedAsync(DateTime before);
}