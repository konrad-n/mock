namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidSelfEducationTitleException : CustomException
{
    public InvalidSelfEducationTitleException(string message) : base(message)
    {
    }
}