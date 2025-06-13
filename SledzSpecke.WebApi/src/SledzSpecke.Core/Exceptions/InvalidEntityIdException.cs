namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidEntityIdException : CustomException
{
    public int Value { get; }

    public InvalidEntityIdException(int value) : base($"Entity ID cannot be less than or equal to 0. Actual value: {value}")
    {
        Value = value;
    }
}