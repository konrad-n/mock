using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class BusinessRuleException : CustomException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}