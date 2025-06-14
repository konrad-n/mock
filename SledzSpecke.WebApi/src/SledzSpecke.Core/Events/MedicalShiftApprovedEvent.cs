using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Events;

/// <summary>
/// Domain event raised when a medical shift is approved
/// </summary>
public sealed record MedicalShiftApprovedEvent(
    MedicalShiftId ShiftId,
    DateTime ApprovedOn,
    string ApprovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}