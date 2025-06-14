using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Events;

/// <summary>
/// Domain event raised when a new medical shift is created
/// </summary>
public sealed record MedicalShiftCreatedEvent(
    MedicalShiftId ShiftId,
    InternshipId InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}