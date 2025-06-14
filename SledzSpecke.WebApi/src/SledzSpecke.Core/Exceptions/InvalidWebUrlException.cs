namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidWebUrlException : CustomException
{
    public InvalidWebUrlException(string message) : base(message)
    {
    }
}