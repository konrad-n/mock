namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPhoneNumberException : CustomException
{
    public InvalidPhoneNumberException(string message) : base(message)
    {
    }
}