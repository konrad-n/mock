namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidDescriptionException : CustomException
{
    public InvalidDescriptionException(string message) : base(message)
    {
    }
}