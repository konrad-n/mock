namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidGenderException : CustomException
{
    public InvalidGenderException(string message) : base(message)
    {
    }
}