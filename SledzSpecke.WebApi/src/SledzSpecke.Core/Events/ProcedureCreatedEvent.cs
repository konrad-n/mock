using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Events;

public sealed record ProcedureCreatedEvent(
    ProcedureId ProcedureId,
    InternshipId InternshipId,
    DateTime Date,
    string Code,
    string Location,
    SmkVersion SmkVersion
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}