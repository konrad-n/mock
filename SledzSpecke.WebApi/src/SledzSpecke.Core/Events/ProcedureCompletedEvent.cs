using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Events;

/// <summary>
/// Domain event raised when a procedure is completed
/// </summary>
public sealed record ProcedureCompletedEvent(
    ProcedureId ProcedureId,
    InternshipId InternshipId,
    string ProcedureName,
    string ProcedureCode,
    int Count,
    DateTime CompletedOn) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}