using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record SpecializationId
{
    public int Value { get; init; }

    private SpecializationId()
    {
        // Required for EF Core
    }

    public SpecializationId(int value)
    {
        if (value <= 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static SpecializationId New() => new(0); // Will be set by EF Core
    public static implicit operator int(SpecializationId specializationId) => specializationId.Value;
    public static implicit operator SpecializationId(int value) => new(value);
}