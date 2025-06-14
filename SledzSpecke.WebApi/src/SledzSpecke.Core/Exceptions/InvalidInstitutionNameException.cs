namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidInstitutionNameException : CustomException
{
    public InvalidInstitutionNameException(string message) : base(message)
    {
    }
}