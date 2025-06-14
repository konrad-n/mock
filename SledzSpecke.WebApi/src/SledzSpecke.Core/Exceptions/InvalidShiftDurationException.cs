namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidShiftDurationException : CustomException
{
    public InvalidShiftDurationException(string message) : base(message)
    {
    }
}