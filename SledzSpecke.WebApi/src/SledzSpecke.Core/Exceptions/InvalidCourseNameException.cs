namespace SledzSpecke.Core.Exceptions;

public sealed class InvalidCourseNameException : CustomException
{
    public InvalidCourseNameException(string message) : base(message)
    {
    }
}