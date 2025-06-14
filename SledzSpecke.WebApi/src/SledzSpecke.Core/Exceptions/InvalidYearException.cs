namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidYearException : CustomException
{
    public InvalidYearException(string message) : base(message)
    {
    }
}