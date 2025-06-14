namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidCreditHoursException : CustomException
{
    public InvalidCreditHoursException(string message) : base(message)
    {
    }
}