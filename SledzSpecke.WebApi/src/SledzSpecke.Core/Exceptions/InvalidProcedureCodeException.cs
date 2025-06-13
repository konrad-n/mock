namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidProcedureCodeException : CustomException
{
    public InvalidProcedureCodeException(string code) : base($"Procedure code '{code}' is invalid.")
    {
    }
}