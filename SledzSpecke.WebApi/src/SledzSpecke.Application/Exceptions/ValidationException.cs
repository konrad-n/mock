using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class ValidationException : CustomException
{
    public ValidationException(string message) : base(message)
    {
    }
}