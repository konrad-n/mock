namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidActivityTitleException : CustomException
{
    public InvalidActivityTitleException(string message) : base(message)
    {
    }
}