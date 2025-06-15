namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPasswordException : CustomException
{
    public InvalidPasswordException() : base("Password is invalid.")
    {
    }

    public InvalidPasswordException(string message) : base(message)
    {
    }
}