namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidISBNException : CustomException
{
    public InvalidISBNException(string message) : base(message)
    {
    }
}