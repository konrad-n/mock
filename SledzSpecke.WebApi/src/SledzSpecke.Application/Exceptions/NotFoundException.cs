using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message) : base(message)
    {
    }
}