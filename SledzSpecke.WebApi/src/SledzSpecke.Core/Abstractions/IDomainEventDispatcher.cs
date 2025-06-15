namespace SledzSpecke.Core.Abstractions;

/// <summary>
/// Defines the contract for dispatching domain events
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}