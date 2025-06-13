using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record ModuleId
{
    public int Value { get; }

    public ModuleId(int value)
    {
        if (value <= 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static implicit operator int(ModuleId moduleId) => moduleId.Value;
    public static implicit operator ModuleId(int value) => new(value);
}