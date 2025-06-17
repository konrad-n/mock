namespace SledzSpecke.Core.DomainEvents.Base;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}