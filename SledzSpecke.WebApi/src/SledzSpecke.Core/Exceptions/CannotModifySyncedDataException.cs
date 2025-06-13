namespace SledzSpecke.Core.Exceptions;

public sealed class CannotModifySyncedDataException : CustomException
{
    public CannotModifySyncedDataException() : base("Cannot modify data that has been synchronized.")
    {
    }
}