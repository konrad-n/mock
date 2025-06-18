using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Sagas;

namespace SledzSpecke.Application.Sagas;

public class SagaOrchestrator<TSaga, TData> : ISagaOrchestrator<TSaga, TData>
    where TSaga : SagaBase<TData>, new()
    where TData : class
{
    private readonly ISagaRepository _sagaRepository;
    private readonly ILogger<SagaOrchestrator<TSaga, TData>> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public SagaOrchestrator(
        ISagaRepository sagaRepository,
        ILogger<SagaOrchestrator<TSaga, TData>> logger,
        IServiceProvider serviceProvider)
    {
        _sagaRepository = sagaRepository;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<Result<Guid>> StartAsync(TData data, CancellationToken cancellationToken)
    {
        try
        {
            var saga = new TSaga();
            await _sagaRepository.SaveAsync(saga, cancellationToken);
            
            _logger.LogInformation("Started saga {SagaType} with ID {SagaId}", 
                typeof(TSaga).Name, saga.Id);
            
            // Execute saga in background
            _ = Task.Run(async () => 
            {
                try
                {
                    await ExecuteSagaAsync(saga, data, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing saga {SagaId}", saga.Id);
                }
            }, cancellationToken);
            
            return Result<Guid>.Success(saga.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start saga {SagaType}", typeof(TSaga).Name);
            return Result<Guid>.Failure($"Failed to start saga: {ex.Message}");
        }
    }
    
    public async Task<Result> ResumeAsync(Guid sagaId, CancellationToken cancellationToken)
    {
        try
        {
            var saga = await _sagaRepository.GetByIdAsync(sagaId, cancellationToken);
            if (saga == null)
            {
                return Result.Failure($"Saga {sagaId} not found");
            }
            
            if (saga.State == SagaState.Completed || saga.State == SagaState.Failed)
            {
                return Result.Failure($"Cannot resume saga in state {saga.State}");
            }
            
            // TODO: Implement resume logic
            // This would require storing which steps have been executed
            // and resuming from the last successful step
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume saga {SagaId}", sagaId);
            return Result.Failure($"Failed to resume saga: {ex.Message}");
        }
    }
    
    public async Task<SagaState> GetStateAsync(Guid sagaId, CancellationToken cancellationToken)
    {
        var saga = await _sagaRepository.GetByIdAsync(sagaId, cancellationToken);
        return saga?.State ?? SagaState.Failed;
    }
    
    private async Task ExecuteSagaAsync(TSaga saga, TData data, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var step in saga.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Saga {SagaId} cancelled", saga.Id);
                    saga.MarkFailed("Saga execution cancelled");
                    await _sagaRepository.UpdateAsync(saga, cancellationToken);
                    return;
                }
                
                _logger.LogInformation("Executing saga step {StepName} for saga {SagaId}", 
                    step.Name, saga.Id);
                
                // Resolve step dependencies if needed
                var stepInstance = ResolveStep(step);
                
                var result = await stepInstance.ExecuteAsync(data, cancellationToken);
                
                if (!result.IsSuccess)
                {
                    _logger.LogError("Saga step {StepName} failed for saga {SagaId}: {Error}", 
                        step.Name, saga.Id, result.Error);
                    
                    await CompensateSagaAsync(saga, data, cancellationToken);
                    return;
                }
                
                saga.RecordStepExecution(step);
                await _sagaRepository.UpdateAsync(saga, cancellationToken);
                
                _logger.LogInformation("Saga step {StepName} completed for saga {SagaId}", 
                    step.Name, saga.Id);
            }
            
            saga.MarkCompleted();
            await _sagaRepository.UpdateAsync(saga, cancellationToken);
            
            _logger.LogInformation("Saga {SagaId} completed successfully", saga.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaId} failed", saga.Id);
            saga.MarkFailed(ex.Message);
            await _sagaRepository.UpdateAsync(saga, cancellationToken);
            
            await CompensateSagaAsync(saga, data, cancellationToken);
        }
    }
    
    private async Task CompensateSagaAsync(TSaga saga, TData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting compensation for saga {SagaId}", saga.Id);
        saga.StartCompensation();
        await _sagaRepository.UpdateAsync(saga, cancellationToken);
        
        var executedSteps = saga.GetExecutedSteps().Reverse();
        
        foreach (var step in executedSteps)
        {
            try
            {
                _logger.LogInformation("Compensating step {StepName} for saga {SagaId}", 
                    step.Name, saga.Id);
                
                var stepInstance = ResolveStep(step);
                var result = await stepInstance.CompensateAsync(data, cancellationToken);
                
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to compensate step {StepName} for saga {SagaId}: {Error}", 
                        step.Name, saga.Id, result.Error);
                    
                    saga.MarkCompensationFailed($"Failed to compensate {step.Name}: {result.Error}");
                    await _sagaRepository.UpdateAsync(saga, cancellationToken);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during compensation of step {StepName} for saga {SagaId}", 
                    step.Name, saga.Id);
                
                saga.MarkCompensationFailed($"Exception during compensation: {ex.Message}");
                await _sagaRepository.UpdateAsync(saga, cancellationToken);
                return;
            }
        }
        
        _logger.LogInformation("Compensation completed for saga {SagaId}", saga.Id);
    }
    
    private ISagaStep<TData> ResolveStep(ISagaStep<TData> step)
    {
        // If the step has dependencies, resolve them from DI
        var stepType = step.GetType();
        var constructors = stepType.GetConstructors();
        
        if (constructors.Length == 0 || constructors[0].GetParameters().Length == 0)
        {
            // No dependencies, return the step as-is
            return step;
        }
        
        // Try to resolve from DI
        var resolvedStep = _serviceProvider.GetService(stepType) as ISagaStep<TData>;
        return resolvedStep ?? step;
    }
}