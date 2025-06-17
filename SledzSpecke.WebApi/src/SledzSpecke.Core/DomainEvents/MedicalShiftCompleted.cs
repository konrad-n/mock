using SledzSpecke.Core.DomainEvents.Base;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainEvents;

public sealed record MedicalShiftCompleted : DomainEvent
{
    public MedicalShiftId ShiftId { get; }
    public InternshipId InternshipId { get; }
    public DateTime Date { get; }
    public ShiftDuration Duration { get; }
    public ShiftType Type { get; }
    public DateTime CompletedAt { get; }

    public MedicalShiftCompleted(
        MedicalShiftId shiftId,
        InternshipId internshipId,
        DateTime date,
        ShiftDuration duration,
        ShiftType type,
        DateTime completedAt)
    {
        ShiftId = shiftId;
        InternshipId = internshipId;
        Date = date;
        Duration = duration;
        Type = type;
        CompletedAt = completedAt;
    }
}