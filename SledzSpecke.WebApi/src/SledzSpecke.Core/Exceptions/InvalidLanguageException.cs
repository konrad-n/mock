namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidLanguageException : CustomException
{
    public InvalidLanguageException(string message) : base(message)
    {
    }
}