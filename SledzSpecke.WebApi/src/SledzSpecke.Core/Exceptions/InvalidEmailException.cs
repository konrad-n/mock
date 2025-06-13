namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidEmailException : CustomException
{
    public InvalidEmailException() : base("Email address is invalid.")
    {
    }
}