namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidThemeException : CustomException
{
    public InvalidThemeException(string message) : base(message)
    {
    }
}