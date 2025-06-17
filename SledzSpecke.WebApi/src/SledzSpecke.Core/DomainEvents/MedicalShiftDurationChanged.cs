using SledzSpecke.Core.DomainEvents.Base;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainEvents;

public sealed record MedicalShiftDurationChanged : DomainEvent
{
    public MedicalShiftId ShiftId { get; }
    public ShiftDuration OldDuration { get; }
    public ShiftDuration NewDuration { get; }

    public MedicalShiftDurationChanged(
        MedicalShiftId shiftId,
        ShiftDuration oldDuration,
        ShiftDuration newDuration)
    {
        ShiftId = shiftId;
        OldDuration = oldDuration;
        NewDuration = newDuration;
    }
}