using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Outbox;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlOutboxRepository : IOutboxRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<OutboxMessage> _outboxMessages;

    public SqlOutboxRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _outboxMessages = context.OutboxMessages;
    }

    public async Task AddAsync(OutboxMessage message)
    {
        await _outboxMessages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100)
    {
        return await _outboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task UpdateAsync(OutboxMessage message)
    {
        _outboxMessages.Update(message);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnprocessedCountAsync()
    {
        return await _outboxMessages.CountAsync(m => m.ProcessedAt == null);
    }

    public async Task DeleteOldProcessedAsync(DateTime before)
    {
        var toDelete = await _outboxMessages
            .Where(m => m.ProcessedAt != null && m.ProcessedAt < before)
            .ToListAsync();

        _outboxMessages.RemoveRange(toDelete);
        await _context.SaveChangesAsync();
    }
}