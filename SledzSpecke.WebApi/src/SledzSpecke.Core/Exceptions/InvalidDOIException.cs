namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidDOIException : CustomException
{
    public InvalidDOIException(string message) : base(message)
    {
    }
}