namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidFilePathException : CustomException
{
    public InvalidFilePathException(string message) : base(message)
    {
    }
}