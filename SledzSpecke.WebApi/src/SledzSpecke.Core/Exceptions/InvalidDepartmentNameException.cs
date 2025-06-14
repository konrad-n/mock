namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidDepartmentNameException : CustomException
{
    public InvalidDepartmentNameException(string message) : base(message)
    {
    }
}