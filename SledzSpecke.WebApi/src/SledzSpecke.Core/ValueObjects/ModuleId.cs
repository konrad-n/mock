using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record ModuleId
{
    public int Value { get; init; }

    private ModuleId()
    {
        // Required for EF Core
    }

    public ModuleId(int value)
    {
        if (value <= 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static ModuleId New() => new(0); // Will be set by EF Core
    public static implicit operator int(ModuleId moduleId) => moduleId.Value;
    public static implicit operator ModuleId(int value) => new(value);
}