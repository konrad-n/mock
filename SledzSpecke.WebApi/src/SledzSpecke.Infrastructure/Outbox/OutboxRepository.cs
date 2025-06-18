using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Outbox;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Outbox;

internal sealed class OutboxRepository : IOutboxRepository
{
    private readonly SledzSpeckeDbContext _context;
    
    public OutboxRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }
    
    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(
        int batchSize = 100, 
        CancellationToken cancellationToken = default)
    {
        // Use raw SQL for reliable row locking
        var sql = @"
            SELECT * FROM outbox.""OutboxMessages""
            WHERE ""ProcessedAt"" IS NULL 
              AND ""RetryCount"" < 3
            ORDER BY ""OccurredAt""
            LIMIT {0}
            FOR UPDATE SKIP LOCKED";
            
        return await _context.OutboxMessages
            .FromSqlRaw(sql, batchSize)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Update(message);
        return Task.CompletedTask;
    }
    
    public async Task<int> GetUnprocessedCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .CountAsync(x => x.ProcessedAt == null && x.RetryCount < 3, cancellationToken);
    }
}