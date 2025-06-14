namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidShiftLocationException : CustomException
{
    public InvalidShiftLocationException(string message) : base(message)
    {
    }
}