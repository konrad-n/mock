using SledzSpecke.Core.ValueObjects.Base;

namespace SledzSpecke.Core.ValueObjects;

public sealed class ProcedureRealizationId : ValueObject
{
    public int Value { get; }

    public ProcedureRealizationId(int value)
    {
        if (value < 0)
            throw new ArgumentException("ProcedureRealizationId must be non-negative", nameof(value));
        Value = value;
    }

    public static implicit operator int(ProcedureRealizationId id) => id.Value;
    public static implicit operator ProcedureRealizationId(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}