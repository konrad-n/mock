namespace SledzSpecke.Core.Exceptions;

public sealed class EmptyEmployeeNumberException : DomainException
{
    public EmptyEmployeeNumberException() 
        : base("Employee number cannot be empty.") 
    {
    }
}