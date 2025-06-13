namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidDateRangeException : CustomException
{
    public InvalidDateRangeException() : base("End date must be after start date.")
    {
    }

    public InvalidDateRangeException(string message) : base(message)
    {
    }
}