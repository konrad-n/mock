namespace SledzSpecke.Core.Abstractions;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}