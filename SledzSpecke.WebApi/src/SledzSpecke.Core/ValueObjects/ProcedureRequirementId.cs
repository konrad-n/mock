using SledzSpecke.Core.ValueObjects.Base;

namespace SledzSpecke.Core.ValueObjects;

public sealed class ProcedureRequirementId : ValueObject
{
    public int Value { get; }

    public ProcedureRequirementId(int value)
    {
        if (value < 0)
            throw new ArgumentException("ProcedureRequirementId must be non-negative", nameof(value));
        Value = value;
    }

    public static implicit operator int(ProcedureRequirementId id) => id.Value;
    public static implicit operator ProcedureRequirementId(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}