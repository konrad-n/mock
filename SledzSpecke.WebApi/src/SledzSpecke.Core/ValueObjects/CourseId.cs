using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record CourseId
{
    public int Value { get; }
    
    public CourseId(int value)
    {
        if (value < 0)
        {
            throw new InvalidEntityIdException(value);
        }
        
        Value = value;
    }
    
    public static CourseId New() => new(0); // Will be set by EF Core
    public static implicit operator int(CourseId courseId) => courseId.Value;
    public static implicit operator CourseId(int courseId) => new(courseId);
}