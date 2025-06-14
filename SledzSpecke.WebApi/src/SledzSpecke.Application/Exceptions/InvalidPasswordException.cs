using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class InvalidPasswordException : CustomException
{
    public InvalidPasswordException(string message) : base(message)
    {
    }
}