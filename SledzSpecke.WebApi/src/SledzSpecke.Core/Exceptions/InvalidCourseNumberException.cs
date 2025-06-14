namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidCourseNumberException : CustomException
{
    public InvalidCourseNumberException(string message) : base(message)
    {
    }
}