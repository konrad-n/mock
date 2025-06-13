namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidFullNameException : CustomException
{
    public InvalidFullNameException() : base("Full name must be between 2 and 200 characters long.")
    {
    }
}