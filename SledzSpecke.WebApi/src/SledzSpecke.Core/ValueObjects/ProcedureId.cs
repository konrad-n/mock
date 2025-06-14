using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ProcedureId
{
    public int Value { get; }

    public ProcedureId(int value)
    {
        if (value < 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static ProcedureId New() => new(0); // Will be set by EF Core
    public static implicit operator int(ProcedureId procedureId) => procedureId.Value;
    public static implicit operator ProcedureId(int procedureId) => new(procedureId);
}