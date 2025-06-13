namespace SledzSpecke.Core.ValueObjects;

public record AbsenceId(Guid Value)
{
    public static AbsenceId New() => new(Guid.NewGuid());
    public static implicit operator Guid(AbsenceId absenceId) => absenceId.Value;
    public static implicit operator AbsenceId(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}