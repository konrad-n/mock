using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Outbox;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlOutboxRepository : Core.Outbox.IOutboxRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<OutboxMessage> _outboxMessages;

    public SqlOutboxRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _outboxMessages = context.OutboxMessages;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _outboxMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _outboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _outboxMessages
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _outboxMessages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetUnprocessedCountAsync(CancellationToken cancellationToken = default)
    {
        return await _outboxMessages.CountAsync(m => m.ProcessedAt == null, cancellationToken);
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