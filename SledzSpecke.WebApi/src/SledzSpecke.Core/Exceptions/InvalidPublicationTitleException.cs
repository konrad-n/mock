namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPublicationTitleException : CustomException
{
    public InvalidPublicationTitleException(string message) : base(message)
    {
    }
}