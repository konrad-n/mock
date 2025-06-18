using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SledzSpecke.Core.Abstractions;

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
    
    protected void AddStep(ISagaStep<TData> step)
    {
        _steps.Add(step);
    }
    
    public IReadOnlyList<ISagaStep<TData>> Steps => _steps.AsReadOnly();
    
    public void RecordStepExecution(ISagaStep<TData> step)
    {
        _executedSteps.Push(step);
        State = SagaState.InProgress;
    }
    
    public void MarkCompleted()
    {
        State = SagaState.Completed;
        CompletedAt = DateTime.UtcNow;
    }
    
    public void MarkFailed(string errorMessage)
    {
        State = SagaState.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
    
    public void StartCompensation()
    {
        State = SagaState.Compensating;
    }
    
    public void MarkCompensationFailed(string errorMessage)
    {
        State = SagaState.CompensationFailed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
    
    public IEnumerable<ISagaStep<TData>> GetExecutedSteps()
    {
        return _executedSteps.ToArray();
    }
}