namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidUsernameException : CustomException
{
    public string? Username { get; }

    public InvalidUsernameException() : base("Username must be between 3 and 50 characters long.")
    {
    }

    public InvalidUsernameException(string message) : base(message)
    {
    }

    public InvalidUsernameException(string username, string message) : base(message)
    {
        Username = username;
    }
}