namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidUserBioException : CustomException
{
    public InvalidUserBioException(string message) : base(message)
    {
    }
}