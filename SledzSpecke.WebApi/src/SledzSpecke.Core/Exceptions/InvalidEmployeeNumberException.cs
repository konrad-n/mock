namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidEmployeeNumberException : DomainException
{
    public InvalidEmployeeNumberException(string employeeNumber) 
        : base($"Employee number '{employeeNumber}' is invalid. Must be 3-20 characters, containing only uppercase letters and numbers.") 
    {
    }
}