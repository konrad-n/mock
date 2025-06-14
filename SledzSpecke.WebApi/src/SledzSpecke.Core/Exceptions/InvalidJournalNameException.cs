namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidJournalNameException : CustomException
{
    public InvalidJournalNameException(string message) : base(message)
    {
    }
}