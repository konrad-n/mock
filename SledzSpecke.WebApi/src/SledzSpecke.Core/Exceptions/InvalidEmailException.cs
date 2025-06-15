namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidEmailException : CustomException
{
    public string? Email { get; }

    public InvalidEmailException() : base("Email address is invalid.")
    {
    }

    public InvalidEmailException(string message) : base(message)
    {
    }

    public InvalidEmailException(string email, string message) : base(message)
    {
        Email = email;
    }
}