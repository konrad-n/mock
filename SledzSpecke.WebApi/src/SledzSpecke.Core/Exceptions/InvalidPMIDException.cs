namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPMIDException : CustomException
{
    public InvalidPMIDException(string message) : base(message)
    {
    }
}