using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Sagas;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Sagas;

public class SagaRepository : ISagaRepository
{
    private readonly SledzSpeckeDbContext _context;
    
    public SagaRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }
    
    public async Task<ISaga?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<SagaStateEntity>()
            .Include(s => s.Steps)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            
        if (entity == null)
            return null;
            
        // For now, we'll return the raw entity data
        // In a full implementation, we'd deserialize based on Type
        return MapFromEntity(entity);
    }
    
    public async Task SaveAsync(ISaga saga, CancellationToken cancellationToken)
    {
        var entity = MapToEntity(saga);
        _context.Set<SagaStateEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(ISaga saga, CancellationToken cancellationToken)
    {
        var existingEntity = await _context.Set<SagaStateEntity>()
            .Include(s => s.Steps)
            .FirstOrDefaultAsync(s => s.Id == saga.Id, cancellationToken);
            
        if (existingEntity == null)
            throw new InvalidOperationException($"Saga {saga.Id} not found");
            
        // Update properties
        existingEntity.State = saga.State.ToString();
        existingEntity.CompletedAt = saga.CompletedAt;
        existingEntity.ErrorMessage = saga.ErrorMessage;
        existingEntity.Data = JsonSerializer.Serialize(saga.Data);
        
        // Update steps
        UpdateSteps(existingEntity, saga);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<SagaStateEntity>()
            .AnyAsync(s => s.Id == id, cancellationToken);
    }
    
    private SagaStateEntity MapToEntity(ISaga saga)
    {
        return new SagaStateEntity
        {
            Id = saga.Id,
            Type = saga.Type,
            State = saga.State.ToString(),
            CreatedAt = saga.CreatedAt,
            CompletedAt = saga.CompletedAt,
            ErrorMessage = saga.ErrorMessage,
            Data = JsonSerializer.Serialize(saga.Data),
            Steps = new List<SagaStepEntity>()
        };
    }
    
    private ISaga MapFromEntity(SagaStateEntity entity)
    {
        // This is a simplified mapping
        // In a full implementation, we'd use the Type to deserialize to the correct saga type
        var saga = new InMemorySaga
        {
            Id = entity.Id,
            Type = entity.Type,
            State = Enum.Parse<SagaState>(entity.State),
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            ErrorMessage = entity.ErrorMessage,
            Data = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.Data) ?? new Dictionary<string, object>()
        };
        
        return saga;
    }
    
    private void UpdateSteps(SagaStateEntity entity, ISaga saga)
    {
        // For now, we'll add new steps
        // In a full implementation, we'd track which steps have been executed
        if (saga is SagaBase<object> baseSaga)
        {
            foreach (var executedStep in baseSaga.GetExecutedSteps())
            {
                if (!entity.Steps.Any(s => s.Name == executedStep.Name))
                {
                    entity.Steps.Add(new SagaStepEntity
                    {
                        Id = Guid.NewGuid(),
                        SagaId = entity.Id,
                        Name = executedStep.Name,
                        Status = "Completed",
                        StartedAt = DateTime.UtcNow,
                        CompletedAt = DateTime.UtcNow
                    });
                }
            }
        }
    }
}

// Temporary implementation for testing
internal class InMemorySaga : ISaga
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public SagaState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}