namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPersonNameException : CustomException
{
    public InvalidPersonNameException(string message) : base(message)
    {
    }
}