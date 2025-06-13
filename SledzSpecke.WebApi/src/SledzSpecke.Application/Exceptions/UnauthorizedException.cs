using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}