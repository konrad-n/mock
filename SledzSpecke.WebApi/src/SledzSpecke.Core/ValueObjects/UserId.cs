using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record UserId
{
    public int Value { get; }

    public UserId(int value)
    {
        if (value <= 0)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }

    public static UserId New() => new(0); // Will be set by EF Core
    public static implicit operator int(UserId userId) => userId.Value;
    public static implicit operator UserId(int value) => new(value);
}