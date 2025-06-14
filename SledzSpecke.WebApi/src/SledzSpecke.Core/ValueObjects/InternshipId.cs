using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record InternshipId
{
    public int Value { get; }

    public InternshipId(int value)
    {
        if (value < 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static InternshipId New() => new(0); // Will be set by EF Core
    public static implicit operator int(InternshipId internshipId) => internshipId.Value;
    public static implicit operator InternshipId(int internshipId) => new(internshipId);
}