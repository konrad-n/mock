using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record SelfEducationTitle
{
    private const int MinLength = 3;
    private const int MaxLength = 500;
    
    public string Value { get; }

    public SelfEducationTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSelfEducationTitleException("Self-education title cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidSelfEducationTitleException($"Self-education title must be at least {MinLength} characters long.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidSelfEducationTitleException($"Self-education title cannot exceed {MaxLength} characters.");
        }

        Value = value;
    }

    public static implicit operator string(SelfEducationTitle title) => title.Value;
    public static implicit operator SelfEducationTitle(string title) => new(title);
    
    public override string ToString() => Value;
}