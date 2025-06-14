namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidCertificateNumberException : CustomException
{
    public InvalidCertificateNumberException(string message) : base(message)
    {
    }
}