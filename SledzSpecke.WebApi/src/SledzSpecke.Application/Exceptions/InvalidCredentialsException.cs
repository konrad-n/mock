using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public sealed class InvalidCredentialsException : CustomException
{
    public InvalidCredentialsException() : base("Invalid credentials provided.")
    {
    }
}