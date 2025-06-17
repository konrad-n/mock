using SledzSpecke.Core.DomainEvents.Base;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainEvents;

public sealed record MedicalShiftCreated : DomainEvent
{
    public MedicalShiftId ShiftId { get; }
    public InternshipId InternshipId { get; }
    public DateTime Date { get; }
    public ShiftDuration Duration { get; }
    public ShiftType Type { get; }
    public string Location { get; }

    public MedicalShiftCreated(
        MedicalShiftId shiftId,
        InternshipId internshipId,
        DateTime date,
        ShiftDuration duration,
        ShiftType type,
        string location)
    {
        ShiftId = shiftId;
        InternshipId = internshipId;
        Date = date;
        Duration = duration;
        Type = type;
        Location = location;
    }
}