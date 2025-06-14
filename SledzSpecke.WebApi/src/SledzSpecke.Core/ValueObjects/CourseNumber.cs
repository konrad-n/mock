using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record CourseNumber
{
    // Course numbers often follow institution-specific formats
    // Common patterns: XXXX-YYYY, CS101, MED-2023-001
    private static readonly Regex CourseNumberPattern = new(@"^[A-Z0-9\-\.\/]{2,50}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public CourseNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCourseNumberException("Course number cannot be empty.");
        }

        value = value.Trim().ToUpperInvariant();

        if (!CourseNumberPattern.IsMatch(value))
        {
            throw new InvalidCourseNumberException(
                "Course number must be 2-50 characters long and contain only uppercase letters, numbers, hyphens, dots, and slashes.");
        }

        Value = value;
    }

    public static implicit operator string(CourseNumber number) => number.Value;
    public static implicit operator CourseNumber(string number) => new(number);
    
    public override string ToString() => Value;
}