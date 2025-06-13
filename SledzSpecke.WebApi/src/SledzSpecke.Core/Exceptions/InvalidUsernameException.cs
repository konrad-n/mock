namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidUsernameException : CustomException
{
    public InvalidUsernameException() : base("Username must be between 3 and 50 characters long.")
    {
    }
}