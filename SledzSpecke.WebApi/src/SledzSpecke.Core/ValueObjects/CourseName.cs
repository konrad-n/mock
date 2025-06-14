using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record CourseName
{
    public string Value { get; }

    public CourseName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCourseNameException("Course name cannot be empty.");
        }

        if (value.Length > 300)
        {
            throw new InvalidCourseNameException("Course name cannot exceed 300 characters.");
        }

        if (value.Length < 3)
        {
            throw new InvalidCourseNameException("Course name must be at least 3 characters long.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(CourseName name) => name.Value;
    public static implicit operator CourseName(string name) => new(name);
    
    public override string ToString() => Value;
}