# SledzSpecke Advanced Patterns Implementation Guide

## Overview

This technical guide provides step-by-step instructions for implementing advanced architectural patterns in SledzSpecke. Based on analysis, the Outbox and Specification patterns are already fully implemented. This guide focuses on:

1. **Saga Pattern** for complex SMK workflows
2. **Enhanced Message Execution Pipeline**
3. **Comprehensive Audit Trail**
4. **Event Sourcing** (optional)

## 1. Saga Pattern Implementation for SMK Workflows

### Purpose
Orchestrate complex, multi-step SMK workflows with compensation logic for handling failures in distributed operations.

### Implementation Steps

#### Step 1: Create Saga Base Infrastructure

```bash
# Create saga infrastructure
sudo mkdir -p src/SledzSpecke.Core/Sagas
sudo mkdir -p src/SledzSpecke.Infrastructure/Sagas
sudo mkdir -p src/SledzSpecke.Application/Sagas
```

#### Step 2: Define Core Saga Abstractions

Create `src/SledzSpecke.Core/Sagas/ISaga.cs`:
```csharp
namespace SledzSpecke.Core.Sagas;

public interface ISaga
{
    Guid Id { get; }
    string Type { get; }
    SagaState State { get; }
    DateTime CreatedAt { get; }
    DateTime? CompletedAt { get; }
    string? ErrorMessage { get; }
    Dictionary<string, object> Data { get; }
}

public enum SagaState
{
    Started,
    InProgress,
    Compensating,
    Completed,
    Failed,
    CompensationFailed
}

public interface ISagaStep<TData>
{
    string Name { get; }
    Task<Result> ExecuteAsync(TData data, CancellationToken cancellationToken);
    Task<Result> CompensateAsync(TData data, CancellationToken cancellationToken);
}

public abstract class SagaBase<TData> : ISaga where TData : class
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public SagaState State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Dictionary<string, object> Data { get; private set; }
    
    private readonly List<ISagaStep<TData>> _steps;
    private readonly Stack<ISagaStep<TData>> _executedSteps;
    
    protected SagaBase(string type)
    {
        Id = Guid.NewGuid();
        Type = type;
        State = SagaState.Started;
        CreatedAt = DateTime.UtcNow;
        Data = new Dictionary<string, object>();
        _steps = new List<ISagaStep<TData>>();
        _executedSteps = new Stack<ISagaStep<TData>>();
    }
}
```

#### Step 3: Implement SMK Monthly Report Saga

Create `src/SledzSpecke.Core/Sagas/SMKMonthlyReportSaga.cs`:
```csharp
namespace SledzSpecke.Core.Sagas;

public class SMKMonthlyReportSagaData
{
    public int InternshipId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<int> ShiftIds { get; set; } = new();
    public List<int> ProcedureIds { get; set; } = new();
    public int? GeneratedReportId { get; set; }
}

public class SMKMonthlyReportSaga : SagaBase<SMKMonthlyReportSagaData>
{
    public SMKMonthlyReportSaga() : base("SMKMonthlyReport")
    {
        AddStep(new ValidateMonthlyHoursStep());
        AddStep(new ValidateProceduresStep());
        AddStep(new GenerateMonthlyReportStep());
        AddStep(new NotifySupervisorStep());
        AddStep(new ArchiveMonthlyDataStep());
    }
}

// Step implementations
public class ValidateMonthlyHoursStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "ValidateMonthlyHours";
    
    private readonly IMedicalShiftRepository _shiftRepository;
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // Validate 160 hours minimum
        // Check weekly limits
        // Return validation result
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // No compensation needed for validation
        return Result.Success();
    }
}
```

#### Step 4: Create Saga Persistence

Create `src/SledzSpecke.Infrastructure/Sagas/SagaState.cs`:
```csharp
namespace SledzSpecke.Infrastructure.Sagas;

public class SagaStateEntity
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string Data { get; set; } // JSON serialized
    public List<SagaStepEntity> Steps { get; set; } = new();
}

public class SagaStepEntity
{
    public Guid Id { get; set; }
    public Guid SagaId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
```

#### Step 5: Implement Saga Orchestrator

Create `src/SledzSpecke.Application/Sagas/SagaOrchestrator.cs`:
```csharp
namespace SledzSpecke.Application.Sagas;

public interface ISagaOrchestrator<TSaga, TData> 
    where TSaga : SagaBase<TData> 
    where TData : class
{
    Task<Result<Guid>> StartAsync(TData data, CancellationToken cancellationToken);
    Task<Result> ResumeAsync(Guid sagaId, CancellationToken cancellationToken);
    Task<SagaState> GetStateAsync(Guid sagaId, CancellationToken cancellationToken);
}

public class SagaOrchestrator<TSaga, TData> : ISagaOrchestrator<TSaga, TData>
    where TSaga : SagaBase<TData>, new()
    where TData : class
{
    private readonly ISagaRepository _sagaRepository;
    private readonly ILogger<SagaOrchestrator<TSaga, TData>> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public async Task<Result<Guid>> StartAsync(TData data, CancellationToken cancellationToken)
    {
        var saga = new TSaga();
        await _sagaRepository.SaveAsync(saga, cancellationToken);
        
        // Execute in background
        _ = Task.Run(async () => await ExecuteSagaAsync(saga, data, cancellationToken));
        
        return Result<Guid>.Success(saga.Id);
    }
    
    private async Task ExecuteSagaAsync(TSaga saga, TData data, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var step in saga.Steps)
            {
                _logger.LogInformation("Executing saga step {StepName} for saga {SagaId}", 
                    step.Name, saga.Id);
                
                var result = await step.ExecuteAsync(data, cancellationToken);
                
                if (!result.IsSuccess)
                {
                    await CompensateSagaAsync(saga, data, cancellationToken);
                    return;
                }
                
                saga.RecordStepExecution(step);
                await _sagaRepository.UpdateAsync(saga, cancellationToken);
            }
            
            saga.MarkCompleted();
            await _sagaRepository.UpdateAsync(saga, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaId} failed", saga.Id);
            await CompensateSagaAsync(saga, data, cancellationToken);
        }
    }
}
```

#### Step 6: Build and Test
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
sudo dotnet build
# Fix any compilation errors before proceeding
```

---

## 2. Enhanced Message Execution Pipeline

### Purpose
Extend the existing pipeline to support complex message processing scenarios with middleware-style execution.

### Implementation Steps

#### Step 1: Create Enhanced Pipeline Abstractions

Create `src/SledzSpecke.Application/Pipeline/IPipelineBuilder.cs`:
```csharp
namespace SledzSpecke.Application.Pipeline;

public interface IPipelineBuilder<TContext>
{
    IPipelineBuilder<TContext> Use(Func<TContext, Func<Task>, Task> middleware);
    IPipelineBuilder<TContext> UseWhen(Predicate<TContext> condition, 
        Func<TContext, Func<Task>, Task> middleware);
    IPipelineBuilder<TContext> UseStep<TStep>() where TStep : IPipelineStep<TContext>;
    Func<TContext, Task> Build();
}

public interface IPipelineStep<TContext>
{
    Task ExecuteAsync(TContext context, Func<Task> next);
}

public class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
{
    private readonly List<Func<TContext, Func<Task>, Task>> _components = new();
    
    public IPipelineBuilder<TContext> Use(Func<TContext, Func<Task>, Task> middleware)
    {
        _components.Add(middleware);
        return this;
    }
    
    public Func<TContext, Task> Build()
    {
        return context =>
        {
            var index = -1;
            
            async Task Next()
            {
                index++;
                if (index < _components.Count)
                {
                    await _components[index](context, Next);
                }
            }
            
            return Next();
        };
    }
}
```

#### Step 2: Create Message Processing Context

Create `src/SledzSpecke.Application/Pipeline/MessageContext.cs`:
```csharp
namespace SledzSpecke.Application.Pipeline;

public class MessageContext
{
    public Guid MessageId { get; set; }
    public string MessageType { get; set; }
    public object Payload { get; set; }
    public Dictionary<string, object> Headers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; }
    public List<string> ExecutionLog { get; set; } = new();
    public bool IsProcessed { get; set; }
    public string? ErrorMessage { get; set; }
    
    public T GetPayload<T>() => (T)Payload;
    
    public void Log(string message)
    {
        ExecutionLog.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} - {message}");
    }
}
```

#### Step 3: Implement Pipeline Steps

Create `src/SledzSpecke.Application/Pipeline/Steps/ValidationStep.cs`:
```csharp
namespace SledzSpecke.Application.Pipeline.Steps;

public class ValidationStep : IPipelineStep<MessageContext>
{
    private readonly IValidator<object> _validator;
    
    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        context.Log($"Validating message {context.MessageId}");
        
        var validationResult = await _validator.ValidateAsync(context.Payload);
        
        if (!validationResult.IsValid)
        {
            context.ErrorMessage = string.Join(", ", validationResult.Errors);
            context.Log($"Validation failed: {context.ErrorMessage}");
            return; // Short-circuit pipeline
        }
        
        context.Log("Validation passed");
        await next();
    }
}

public class RetryStep : IPipelineStep<MessageContext>
{
    private readonly IRetryPolicy _retryPolicy;
    
    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                context.RetryCount++;
                context.Log($"Retry {context.RetryCount} failed: {ex.Message}");
                throw;
            }
        });
    }
}

public class DeadLetterStep : IPipelineStep<MessageContext>
{
    private readonly IDeadLetterService _deadLetterService;
    
    public async Task ExecuteAsync(MessageContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            if (context.RetryCount >= 3)
            {
                context.Log($"Moving to dead letter: {ex.Message}");
                await _deadLetterService.MoveToDeadLetterAsync(context);
                return;
            }
            throw;
        }
    }
}
```

#### Step 4: Create Pipeline Factory

Create `src/SledzSpecke.Application/Pipeline/PipelineFactory.cs`:
```csharp
namespace SledzSpecke.Application.Pipeline;

public interface IPipelineFactory
{
    Func<MessageContext, Task> CreatePipeline(string messageType);
}

public class PipelineFactory : IPipelineFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IPipelineBuilder<MessageContext>, IPipelineBuilder<MessageContext>>> _configurations = new();
    
    public PipelineFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RegisterPipelines();
    }
    
    private void RegisterPipelines()
    {
        // Medical shift pipeline
        _configurations["MedicalShift"] = builder => builder
            .UseStep<ValidationStep>()
            .UseStep<DeadLetterStep>()
            .UseStep<RetryStep>()
            .Use(async (context, next) =>
            {
                // Custom medical shift logic
                context.Log("Processing medical shift");
                await next();
            })
            .UseStep<OutboxStep>();
            
        // Procedure pipeline
        _configurations["Procedure"] = builder => builder
            .UseStep<ValidationStep>()
            .UseWhen(ctx => ctx.Headers.ContainsKey("Priority") && ctx.Headers["Priority"].Equals("High"),
                async (context, next) =>
                {
                    context.Log("High priority processing");
                    await next();
                })
            .UseStep<OutboxStep>();
    }
    
    public Func<MessageContext, Task> CreatePipeline(string messageType)
    {
        var builder = new PipelineBuilder<MessageContext>();
        
        if (_configurations.TryGetValue(messageType, out var configure))
        {
            configure(builder);
        }
        else
        {
            // Default pipeline
            builder
                .UseStep<ValidationStep>()
                .UseStep<OutboxStep>();
        }
        
        return builder.Build();
    }
}
```

#### Step 5: Build and Test
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
sudo dotnet build
# Fix any compilation errors
```

---

## 3. Comprehensive Audit Trail Implementation

### Purpose
Track all changes to critical entities with full history and change tracking.

### Implementation Steps

#### Step 1: Create Audit Infrastructure

Create `src/SledzSpecke.Core/Auditing/IAuditable.cs`:
```csharp
namespace SledzSpecke.Core.Auditing;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    string CreatedBy { get; }
    DateTime? ModifiedAt { get; }
    string? ModifiedBy { get; }
}

public interface IAuditLog
{
    Guid Id { get; }
    string EntityType { get; }
    string EntityId { get; }
    string Action { get; }
    string UserId { get; }
    DateTime Timestamp { get; }
    string? OldValues { get; }
    string? NewValues { get; }
    string? PropertyName { get; }
}

public class AuditLog : IAuditLog
{
    public Guid Id { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string Action { get; private set; }
    public string UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? PropertyName { get; private set; }
    
    private AuditLog() { } // For EF
    
    public static AuditLog Create(string entityType, string entityId, string action, 
        string userId, object? oldValues = null, object? newValues = null, string? propertyName = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            PropertyName = propertyName
        };
    }
}
```

#### Step 2: Create Audit Interceptor

Create `src/SledzSpecke.Infrastructure/Auditing/AuditInterceptor.cs`:
```csharp
namespace SledzSpecke.Infrastructure.Auditing;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly List<AuditLog> _auditLogs = new();
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ProcessAuditableEntities(eventData.Context);
            GenerateAuditLogs(eventData.Context);
        }
        
        return base.SavingChanges(eventData, result);
    }
    
    private void ProcessAuditableEntities(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));
        
        foreach (var entry in entries)
        {
            var auditable = (IAuditable)entry.Entity;
            var now = DateTime.UtcNow;
            var user = _currentUserService.UserId ?? "System";
            
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.ModifiedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.ModifiedBy)).CurrentValue = user;
                entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
            }
        }
    }
    
    private void GenerateAuditLogs(DbContext context)
    {
        var auditableEntityTypes = new[] { "MedicalShift", "Procedure", "Internship", "User" };
        
        var entries = context.ChangeTracker.Entries()
            .Where(e => auditableEntityTypes.Contains(e.Entity.GetType().Name) &&
                       (e.State == EntityState.Added || 
                        e.State == EntityState.Modified || 
                        e.State == EntityState.Deleted));
        
        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);
            var action = entry.State.ToString();
            var userId = _currentUserService.UserId ?? "System";
            
            if (entry.State == EntityState.Modified)
            {
                foreach (var property in entry.Properties.Where(p => p.IsModified))
                {
                    var auditLog = AuditLog.Create(
                        entityType,
                        entityId,
                        "Modified",
                        userId,
                        property.OriginalValue,
                        property.CurrentValue,
                        property.Metadata.Name
                    );
                    
                    _auditLogs.Add(auditLog);
                }
            }
            else
            {
                var auditLog = AuditLog.Create(
                    entityType,
                    entityId,
                    action,
                    userId,
                    entry.State == EntityState.Deleted ? entry.Entity : null,
                    entry.State == EntityState.Added ? entry.Entity : null
                );
                
                _auditLogs.Add(auditLog);
            }
        }
    }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ProcessAuditableEntities(eventData.Context);
            GenerateAuditLogs(eventData.Context);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is not null && _auditLogs.Any())
        {
            // Save audit logs
            eventData.Context.Set<AuditLog>().AddRange(_auditLogs);
            _auditLogs.Clear();
            eventData.Context.SaveChanges();
        }
        
        return base.SavedChanges(eventData, result);
    }
}
```

#### Step 3: Configure Audit in DbContext

Update `src/SledzSpecke.Infrastructure/DAL/SledzSpeckeDbContext.cs`:
```csharp
// Add to OnConfiguring or in service registration
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.AddInterceptors(new AuditInterceptor(_currentUserService));
}

// Add AuditLog configuration
public DbSet<AuditLog> AuditLogs { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Existing configurations...
    
    modelBuilder.Entity<AuditLog>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => new { e.EntityType, e.EntityId });
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.Timestamp);
        entity.Property(e => e.OldValues).HasColumnType("jsonb");
        entity.Property(e => e.NewValues).HasColumnType("jsonb");
    });
}
```

#### Step 4: Create Migration and Build
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
sudo dotnet ef migrations add AddAuditLogging -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
sudo dotnet build
# Apply migration if build succeeds
sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

---

## 4. Event Sourcing Implementation (Optional)

### Purpose
Implement event sourcing for critical aggregates that require complete history and temporal queries.

### Implementation Steps

#### Step 1: Create Event Store Infrastructure

Create `src/SledzSpecke.Core/EventSourcing/IEventSourced.cs`:
```csharp
namespace SledzSpecke.Core.EventSourcing;

public interface IEventSourced
{
    Guid Id { get; }
    int Version { get; }
    IReadOnlyList<IDomainEvent> GetUncommittedEvents();
    void MarkEventsAsCommitted();
    void LoadFromHistory(IEnumerable<IDomainEvent> history);
}

public abstract class EventSourcedAggregate : AggregateRoot, IEventSourced
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();
    
    public int Version { get; private set; }
    
    public IReadOnlyList<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
    
    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }
    
    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            ApplyEvent(@event, false);
            Version++;
        }
    }
    
    protected void RaiseEvent(IDomainEvent @event)
    {
        ApplyEvent(@event, true);
        _uncommittedEvents.Add(@event);
    }
    
    private void ApplyEvent(IDomainEvent @event, bool isNew)
    {
        var method = GetType().GetMethod("Apply", 
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, 
            new[] { @event.GetType() }, 
            null);
            
        if (method == null)
        {
            throw new InvalidOperationException(
                $"Apply method not found for event type {@event.GetType().Name}");
        }
        
        method.Invoke(this, new object[] { @event });
        
        if (isNew)
        {
            Version++;
        }
    }
}
```

#### Step 2: Create Event Store

Create `src/SledzSpecke.Infrastructure/EventSourcing/EventStore.cs`:
```csharp
namespace SledzSpecke.Infrastructure.EventSourcing;

public interface IEventStore
{
    Task<IEnumerable<EventData>> GetEventsAsync(Guid aggregateId, int fromVersion = 0);
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);
    Task<T?> GetAggregateAsync<T>(Guid id) where T : EventSourcedAggregate, new();
    Task SaveAggregateAsync<T>(T aggregate) where T : EventSourcedAggregate;
}

public class EventData
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; }
    public string EventData { get; set; }
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
}

public class PostgresEventStore : IEventStore
{
    private readonly SledzSpeckeDbContext _context;
    private readonly IEventSerializer _serializer;
    private readonly ICurrentUserService _currentUserService;
    
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
    {
        var eventsList = events.ToList();
        if (!eventsList.Any()) return;
        
        // Check for concurrency
        var currentVersion = await _context.Set<EventData>()
            .Where(e => e.AggregateId == aggregateId)
            .MaxAsync(e => (int?)e.Version) ?? 0;
            
        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException($"Expected version {expectedVersion} but was {currentVersion}");
        }
        
        var version = expectedVersion;
        foreach (var @event in eventsList)
        {
            version++;
            var eventData = new EventData
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateType = @event.GetType().Assembly.GetName().Name,
                Version = version,
                EventType = @event.GetType().Name,
                EventData = _serializer.Serialize(@event),
                Timestamp = DateTime.UtcNow,
                UserId = _currentUserService.UserId
            };
            
            _context.Set<EventData>().Add(eventData);
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<T?> GetAggregateAsync<T>(Guid id) where T : EventSourcedAggregate, new()
    {
        var events = await GetEventsAsync(id);
        if (!events.Any()) return null;
        
        var aggregate = new T();
        var domainEvents = events.Select(e => _serializer.Deserialize(e.EventData, Type.GetType(e.EventType)));
        aggregate.LoadFromHistory(domainEvents);
        
        return aggregate;
    }
}
```

#### Step 3: Implement Event-Sourced Medical Shift

Create `src/SledzSpecke.Core/EventSourcing/EventSourcedMedicalShift.cs`:
```csharp
namespace SledzSpecke.Core.EventSourcing;

public class EventSourcedMedicalShift : EventSourcedAggregate
{
    public int InternshipId { get; private set; }
    public LocalDate Date { get; private set; }
    public string Location { get; private set; }
    public Duration Duration { get; private set; }
    public MedicalShiftStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    
    private EventSourcedMedicalShift() { } // For loading from events
    
    public static EventSourcedMedicalShift Create(
        int internshipId, 
        LocalDate date, 
        string location, 
        Duration duration)
    {
        var shift = new EventSourcedMedicalShift();
        shift.RaiseEvent(new MedicalShiftCreatedEvent(
            Guid.NewGuid(), 
            internshipId, 
            date, 
            location, 
            duration));
        return shift;
    }
    
    public Result Approve()
    {
        if (Status != MedicalShiftStatus.Pending)
            return Result.Failure("Can only approve pending shifts");
            
        RaiseEvent(new MedicalShiftApprovedEvent(Id));
        return Result.Success();
    }
    
    public Result Reject(string reason)
    {
        if (Status != MedicalShiftStatus.Pending)
            return Result.Failure("Can only reject pending shifts");
            
        RaiseEvent(new MedicalShiftRejectedEvent(Id, reason));
        return Result.Success();
    }
    
    // Event handlers
    private void Apply(MedicalShiftCreatedEvent @event)
    {
        Id = @event.ShiftId;
        InternshipId = @event.InternshipId;
        Date = @event.Date;
        Location = @event.Location;
        Duration = @event.Duration;
        Status = MedicalShiftStatus.Pending;
    }
    
    private void Apply(MedicalShiftApprovedEvent @event)
    {
        Status = MedicalShiftStatus.Approved;
    }
    
    private void Apply(MedicalShiftRejectedEvent @event)
    {
        Status = MedicalShiftStatus.Rejected;
        RejectionReason = @event.Reason;
    }
}
```

#### Step 4: Create Snapshot Support

Create `src/SledzSpecke.Infrastructure/EventSourcing/SnapshotStore.cs`:
```csharp
namespace SledzSpecke.Infrastructure.EventSourcing;

public interface ISnapshotStore
{
    Task<Snapshot?> GetLatestSnapshotAsync(Guid aggregateId);
    Task SaveSnapshotAsync(Guid aggregateId, object aggregateState, int version);
}

public class Snapshot
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
    public string Data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PostgresSnapshotStore : ISnapshotStore
{
    private readonly SledzSpeckeDbContext _context;
    private readonly IEventSerializer _serializer;
    
    public async Task SaveSnapshotAsync(Guid aggregateId, object aggregateState, int version)
    {
        // Save snapshot every 10 events
        if (version % 10 != 0) return;
        
        var snapshot = new Snapshot
        {
            Id = Guid.NewGuid(),
            AggregateId = aggregateId,
            Version = version,
            Data = _serializer.Serialize(aggregateState),
            Timestamp = DateTime.UtcNow
        };
        
        _context.Set<Snapshot>().Add(snapshot);
        await _context.SaveChangesAsync();
    }
}
```

#### Step 5: Create Projections

Create `src/SledzSpecke.Application/EventSourcing/Projections/MedicalShiftProjection.cs`:
```csharp
namespace SledzSpecke.Application.EventSourcing.Projections;

public class MedicalShiftProjection : 
    INotificationHandler<MedicalShiftCreatedEvent>,
    INotificationHandler<MedicalShiftApprovedEvent>,
    INotificationHandler<MedicalShiftRejectedEvent>
{
    private readonly IMedicalShiftReadModelRepository _repository;
    
    public async Task Handle(MedicalShiftCreatedEvent notification, CancellationToken cancellationToken)
    {
        var readModel = new MedicalShiftReadModel
        {
            Id = notification.ShiftId,
            InternshipId = notification.InternshipId,
            Date = notification.Date,
            Location = notification.Location,
            Duration = notification.Duration,
            Status = "Pending"
        };
        
        await _repository.SaveAsync(readModel, cancellationToken);
    }
    
    public async Task Handle(MedicalShiftApprovedEvent notification, CancellationToken cancellationToken)
    {
        var readModel = await _repository.GetByIdAsync(notification.ShiftId, cancellationToken);
        if (readModel != null)
        {
            readModel.Status = "Approved";
            readModel.ApprovedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(readModel, cancellationToken);
        }
    }
}
```

#### Step 6: Build Everything
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
sudo dotnet build

# If successful, create migrations
sudo dotnet ef migrations add AddEventSourcing -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Apply if needed
sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

---

## Testing Strategy

### Unit Tests for Each Pattern

#### Saga Tests
```csharp
[Fact]
public async Task SMKMonthlyReportSaga_Should_Complete_Successfully()
{
    // Arrange
    var sagaOrchestrator = new SagaOrchestrator<SMKMonthlyReportSaga, SMKMonthlyReportSagaData>(
        _sagaRepository, _logger, _serviceProvider);
    
    var data = new SMKMonthlyReportSagaData
    {
        InternshipId = 1,
        Year = 2024,
        Month = 1
    };
    
    // Act
    var result = await sagaOrchestrator.StartAsync(data, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    // Wait for saga completion
    await Task.Delay(5000);
    var state = await sagaOrchestrator.GetStateAsync(result.Value, CancellationToken.None);
    state.Should().Be(SagaState.Completed);
}
```

#### Pipeline Tests
```csharp
[Fact]
public async Task Pipeline_Should_Execute_Steps_In_Order()
{
    // Arrange
    var context = new MessageContext
    {
        MessageId = Guid.NewGuid(),
        MessageType = "MedicalShift",
        Payload = new CreateMedicalShiftCommand(1, DateTime.Now, 8, "Hospital")
    };
    
    var pipeline = _pipelineFactory.CreatePipeline("MedicalShift");
    
    // Act
    await pipeline(context);
    
    // Assert
    context.IsProcessed.Should().BeTrue();
    context.ExecutionLog.Should().Contain(log => log.Contains("Validation passed"));
    context.ExecutionLog.Should().Contain(log => log.Contains("Processing medical shift"));
}
```

---

## Deployment Instructions

### Step-by-Step Deployment

1. **Build and Test Locally**
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
sudo dotnet build
sudo dotnet test
```

2. **Run E2E Tests**
```bash
./run-e2e-tests-isolated.sh
```

3. **Deploy to Production**
```bash
# Commit changes
sudo git add .
sudo git commit -m "feat: Implement Saga, Enhanced Pipeline, Audit Trail, and Event Sourcing patterns"
sudo git push origin master

# The GitHub Actions will automatically deploy
# Monitor deployment
./check-builds.sh latest
```

4. **Verify Production**
```bash
# Check API status
sudo systemctl status sledzspecke-api

# Monitor logs
sudo journalctl -u sledzspecke-api -f

# Check monitoring dashboard
# https://api.sledzspecke.pl/monitoring/dashboard
```

---

## Performance Considerations

1. **Saga Execution**: Run in background tasks to avoid blocking requests
2. **Event Store**: Implement snapshots every 10-50 events
3. **Audit Logs**: Use PostgreSQL JSONB for efficient querying
4. **Pipeline**: Cache pipeline configurations for performance

---

## Security Notes

1. **Audit Trail**: Ensures all changes are tracked with user information
2. **Event Store**: Immutable log provides tamper-proof history
3. **Saga Compensation**: Ensures data consistency in failure scenarios
4. **Dead Letter Queue**: Prevents message loss

---

## Monitoring and Observability

1. **Saga Dashboard**: View running sagas at `/monitoring/sagas`
2. **Pipeline Metrics**: Track execution times and failure rates
3. **Audit Query API**: Search audit logs by entity, user, or date
4. **Event Store Browser**: View event streams for debugging

---

## Common Issues and Solutions

### Issue: Saga Step Timeout
**Solution**: Increase timeout in saga configuration or implement async processing

### Issue: Event Store Performance
**Solution**: Implement read models and projections for queries

### Issue: Pipeline Memory Usage
**Solution**: Implement streaming for large messages

### Issue: Audit Log Growth
**Solution**: Implement archival strategy for old audit logs

---

## References

- [Saga Pattern Documentation](https://microservices.io/patterns/data/saga.html)
- [Event Sourcing Best Practices](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [ASP.NET Core Pipeline Pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware)
- [Audit Trail Implementation Guide](https://docs.microsoft.com/en-us/ef/core/saving/interceptors)