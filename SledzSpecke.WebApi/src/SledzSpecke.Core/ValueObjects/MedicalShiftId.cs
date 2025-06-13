using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record MedicalShiftId
{
    public int Value { get; }
    
    public MedicalShiftId(int value)
    {
        if (value < 0)
        {
            throw new InvalidEntityIdException(value);
        }
        
        Value = value;
    }
    
    public static MedicalShiftId New() => new(0); // Will be set by EF Core
    public static implicit operator int(MedicalShiftId medicalShiftId) => medicalShiftId.Value;
    public static implicit operator MedicalShiftId(int medicalShiftId) => new(medicalShiftId);
}