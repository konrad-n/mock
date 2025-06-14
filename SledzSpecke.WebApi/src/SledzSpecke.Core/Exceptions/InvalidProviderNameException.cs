namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidProviderNameException : CustomException
{
    public InvalidProviderNameException(string message) : base(message)
    {
    }
}