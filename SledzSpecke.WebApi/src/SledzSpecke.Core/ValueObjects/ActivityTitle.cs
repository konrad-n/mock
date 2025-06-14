using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ActivityTitle
{
    private const int MinLength = 3;
    private const int MaxLength = 200;
    
    public string Value { get; }

    public ActivityTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidActivityTitleException("Activity title cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidActivityTitleException($"Activity title must be at least {MinLength} characters long.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidActivityTitleException($"Activity title cannot exceed {MaxLength} characters.");
        }

        Value = value;
    }

    public static implicit operator string(ActivityTitle title) => title.Value;
    public static implicit operator ActivityTitle(string title) => new(title);
    
    public override string ToString() => Value;
}