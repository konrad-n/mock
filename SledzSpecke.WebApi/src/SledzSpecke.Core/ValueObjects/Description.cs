using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Description
{
    private const int MaxLength = 2000;
    
    public string Value { get; }

    public Description(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = string.Empty;
            return;
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidDescriptionException($"Description cannot exceed {MaxLength} characters.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(Description description) => description.Value;
    public static implicit operator Description(string? description) => new(description);
    
    public override string ToString() => Value;
    
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public int Length => Value.Length;
}