namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidPointsException : DomainException
{
    public InvalidPointsException(decimal points) 
        : base($"Points value '{points}' is invalid. Must be between 0 and 1000.") 
    {
    }
}