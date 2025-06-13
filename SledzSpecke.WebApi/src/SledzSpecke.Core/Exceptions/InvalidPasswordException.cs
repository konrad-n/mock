namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPasswordException : CustomException
{
    public InvalidPasswordException() : base("Password must be at least 6 characters long.")
    {
    }
}